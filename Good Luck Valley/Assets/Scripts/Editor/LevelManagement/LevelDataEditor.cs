using System;
using System.Collections.Generic;
using System.IO;
using GoodLuckValley.Core.Utilities;
using GoodLuckValley.World.LevelManagement.Data;
using UnityEditor;
using UnityEngine;

namespace GoodLuckValley.Editor.LevelManagement
{
    /// <summary>
    /// Custom inspector for <see cref="LevelData"/>. Provides a manual bake
    /// button, a read-only table of baked spawn point data, stale-data warnings,
    /// and validation for scene ID and spawn point configuration.
    /// </summary>
    [CustomEditor(typeof(LevelData))]
    public class LevelDataEditor : UnityEditor.Editor
    {
        private bool _showBakedSpawnPoints = true;
        
        private SerializedProperty _sceneIDProp;
        private SerializedProperty _displayNameProp;
        private SerializedProperty _areaIDProp;
        private SerializedProperty _stableIDProp;
        private SerializedProperty _defaultEntryTransitionProp;
        
        private void OnEnable()
        {
            _sceneIDProp = serializedObject.FindProperty("sceneID");
            _displayNameProp = serializedObject.FindProperty("displayName");
            _areaIDProp = serializedObject.FindProperty("areaID");
            _stableIDProp = serializedObject.FindProperty("stableID");
            _defaultEntryTransitionProp = serializedObject.FindProperty("defaultEntryTransition");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Scene ID — track changes for auto-computing stable ID
            string previousSceneID = _sceneIDProp.stringValue;
            EditorGUILayout.PropertyField(_sceneIDProp);

            if (_sceneIDProp.stringValue != previousSceneID && !string.IsNullOrEmpty(_sceneIDProp.stringValue))
                _stableIDProp.intValue = HashUtility.ComputeStableHash(_sceneIDProp.stringValue);

            // Stable ID (read-only, auto-computed)
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(_stableIDProp);
            EditorGUI.EndDisabledGroup();

            EditorGUILayout.PropertyField(_displayNameProp);
            EditorGUILayout.PropertyField(_areaIDProp);
            EditorGUILayout.PropertyField(_defaultEntryTransitionProp);

            // bakedSpawnPoints and lastBakeTimestamp are intentionally not drawn here —
            // baked data is shown in the read-only table below, and the timestamp is internal.

            serializedObject.ApplyModifiedProperties();

            LevelData data = (LevelData)target;

            // Bake Controls
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Spawn Point Baking", EditorStyles.boldLabel);

            DrawStaleWarning(data);

            bool hasSceneID = !string.IsNullOrEmpty(data.SceneID);

            EditorGUI.BeginDisabledGroup(!hasSceneID);

            if (GUILayout.Button("Bake Spawn Points"))
                SpawnPointBaker.BakeSpawnPoints(data);

            EditorGUI.EndDisabledGroup();

            if (!hasSceneID)
            {
                EditorGUILayout.HelpBox(
                    "Set a Scene ID before baking spawn points.",
                    MessageType.Info
                );
            }

            // Baked Data Table
            DrawBakedSpawnPointTable(data);

            // Validation
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Validation", EditorStyles.boldLabel);

            DrawValidation(data);
        }

        /// <summary>
        /// Displays a warning in the Unity Editor if the baked spawn point data for the given level
        /// is stale, meaning the associated scene file has been modified since the last bake timestamp.
        /// </summary>
        /// <param name="data">The level data object to be checked for stale spawn point information.</param>
        private void DrawStaleWarning(LevelData data)
        {
            if (string.IsNullOrEmpty(data.SceneID))
                return;

            string scenePath = SpawnPointBaker.FindSceneAssetPath(data.SceneID);

            if (string.IsNullOrEmpty(scenePath))
                return;

            string fullPath = Path.GetFullPath(scenePath);

            if (!File.Exists(fullPath))
                return;

            DateTime sceneModTimeUtc = File.GetLastWriteTimeUtc(fullPath);
            long sceneModTimestamp = new DateTimeOffset(sceneModTimeUtc).ToUnixTimeMilliseconds();

            if (sceneModTimestamp > data.LastBakeTimestamp)
            {
                EditorGUILayout.HelpBox(
                    "Baked spawn point data may be stale. The scene file has been "
                    + "modified since the last bake.",
                    MessageType.Warning
                );
            }
        }

        /// <summary>
        /// Draws a table displaying information about the baked spawn points associated with the provided level data.
        /// This includes details such as the ID, position, and facing direction of each spawn point.
        /// </summary>
        /// <param name="data">The level data object containing the baked spawn point information to be displayed.</param>
        private void DrawBakedSpawnPointTable(LevelData data)
        {
            IReadOnlyList<BakedSpawnPoint> bakedPoints = data.BakedSpawnPoints;

            EditorGUILayout.Space(4);
            _showBakedSpawnPoints = EditorGUILayout.Foldout(
                _showBakedSpawnPoints,
                $"Baked Spawn Points ({bakedPoints.Count})",
                true
            );

            if (!_showBakedSpawnPoints || bakedPoints.Count == 0)
                return;

            EditorGUI.indentLevel++;

            // Header row
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("ID", EditorStyles.boldLabel, GUILayout.MinWidth(120));
            EditorGUILayout.LabelField("Position", EditorStyles.boldLabel, GUILayout.MinWidth(100));
            EditorGUILayout.LabelField("Facing", EditorStyles.boldLabel, GUILayout.Width(50));
            EditorGUILayout.EndHorizontal();

            // Data rows
            for (int i = 0; i < bakedPoints.Count; i++)
            {
                BakedSpawnPoint point = bakedPoints[i];

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(point.ID, GUILayout.MinWidth(120));
                EditorGUILayout.LabelField(
                    $"({point.Position.x:F1}, {point.Position.y:F1})",
                    GUILayout.MinWidth(100)
                );
                EditorGUILayout.LabelField(
                    point.FaceRight ? "\u2192" : "\u2190",
                    GUILayout.Width(50)
                );
                EditorGUILayout.EndHorizontal();
            }

            EditorGUI.indentLevel--;
        }

        private void DrawValidation(LevelData data)
        {
            bool hasErrors = false;

            if (string.IsNullOrEmpty(data.SceneID))
            {
                EditorGUILayout.HelpBox(
                    "Scene ID is empty. This level cannot be loaded.",
                    MessageType.Error
                );

                hasErrors = true;
            }

            if (data.BakedSpawnPoints.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "No baked spawn points. Level transitions targeting "
                    + "this level may fail to find a spawn point.",
                    MessageType.Warning
                );
            }

            // Check for duplicate or empty spawn point IDs
            HashSet<string> seenIds = new HashSet<string>();
            IReadOnlyList<BakedSpawnPoint> bakedSpawnPoints = data.BakedSpawnPoints;

            for (int i = 0; i < bakedSpawnPoints.Count; i++)
            {
                string id = bakedSpawnPoints[i].ID;

                if (string.IsNullOrEmpty(id))
                {
                    EditorGUILayout.HelpBox(
                        $"Baked spawn point ID at index {i} is empty.",
                        MessageType.Error
                    );

                    hasErrors = true;
                    continue;
                }

                if (!seenIds.Add(id))
                {
                    EditorGUILayout.HelpBox(
                        $"Duplicate baked spawn point ID '{id}' at index {i}.",
                        MessageType.Error
                    );

                    hasErrors = true;
                }
            }

            if (!hasErrors)
            {
                EditorGUILayout.HelpBox(
                    "Level configuration valid.",
                    MessageType.Info
                );
            }
        }
    }
}
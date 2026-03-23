using System;
using System.Collections.Generic;
using GoodLuckValley.World.LevelManagement.Adapters;
using GoodLuckValley.World.LevelManagement.Data;
using UnityEditor;
using UnityEngine;

namespace GoodLuckValley.Editor.LevelManagement
{
    /// <summary>
    /// Custom inspector for <see cref="LevelTransitionTrigger"/>. Draws dropdown
    /// selectors for the target level (from <see cref="LevelRegistry"/>) and spawn
    /// point (from <see cref="BakedSpawnPoint"/> data on the selected level).
    /// Validates Collider2D trigger setup and required field configuration.
    /// </summary>
    [CustomEditor(typeof(LevelTransitionTrigger))]
    public class LevelTransitionTriggerEditor : UnityEditor.Editor
    {
        private SerializedProperty _targetLevelProp;
        private SerializedProperty _targetSpawnPointIDProp;
        private SerializedProperty _transitionConfigProp;

        private LevelRegistry _levelRegistry;
        private string[] _levelDisplayNames;
        private LevelData[] _levelDataArray;

        private void OnEnable()
        {
            _targetLevelProp = serializedObject.FindProperty("targetLevel");
            _targetSpawnPointIDProp = serializedObject.FindProperty("targetSpawnPointID");
            _transitionConfigProp = serializedObject.FindProperty("transitionConfig");

            CacheLevelRegistry();
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Target", EditorStyles.boldLabel);

            // Target Level dropdown
            DrawLevelDropdown();

            // Target Spawn Point dropdown
            LevelData currentLevel = (LevelData)_targetLevelProp.objectReferenceValue;
            DrawSpawnPointDropdown(currentLevel);

            EditorGUILayout.Space(4);
            EditorGUILayout.PropertyField(_transitionConfigProp);

            serializedObject.ApplyModifiedProperties();

            // Validation
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Validation", EditorStyles.boldLabel);
            DrawValidation();
        }

        /// <summary>
        /// Draws a dropdown menu to select the target level for a level transition.
        /// The dropdown options are dynamically populated based on the levels available
        /// in the project's LevelRegistry asset. Ensures a fallback method for level selection
        /// if no LevelRegistry is configured or available.
        /// </summary>
        private void DrawLevelDropdown()
        {
            if (!_levelRegistry)
            {
                EditorGUILayout.HelpBox(
                    "No LevelRegistry asset found in the project. "
                    + "Create one via Good Luck Valley > Levels > Level Registry.",
                    MessageType.Warning
                );

                // Fall back to default object field
                EditorGUILayout.PropertyField(_targetLevelProp);
                return;
            }

            LevelData currentLevel = (LevelData)_targetLevelProp.objectReferenceValue;
            int currentIndex = FindLevelIndex(currentLevel);
            int newIndex = EditorGUILayout.Popup("Target Level", currentIndex, _levelDisplayNames);

            if (newIndex != currentIndex)
            {
                _targetLevelProp.objectReferenceValue = newIndex > 0 
                    ? _levelDataArray[newIndex - 1] 
                    : null;
            }
        }

        /// <summary>
        /// Draws a dropdown menu for selecting a target spawn point based on the provided level's baked spawn points.
        /// The dropdown populates dynamically depending on the available spawn points in the specified level.
        /// </summary>
        /// <param name="currentLevel">The level containing the baked spawn points to populate the dropdown.
        /// If null or no spawn points are available, the dropdown will be disabled or display appropriate messages.</param>
        private void DrawSpawnPointDropdown(LevelData currentLevel)
        {
            if (!currentLevel)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Popup(
                    "Target Spawn Point", 0,
                    new string[] { "(Select a target level first)" }
                );
                EditorGUI.EndDisabledGroup();
                return;
            }

            IReadOnlyList<BakedSpawnPoint> bakedPoints = currentLevel.BakedSpawnPoints;

            if (bakedPoints.Count == 0)
            {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.Popup(
                    "Target Spawn Point", 0,
                    new string[] { "(No baked spawn points)" }
                );
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.HelpBox(
                    "Bake spawn points on the target LevelData first.",
                    MessageType.Warning
                );
                return;
            }

            // Build spawn point names array
            string[] spawnPointNames = new string[bakedPoints.Count + 1];
            spawnPointNames[0] = "(None)";

            for (int i = 0; i < bakedPoints.Count; i++)
            {
                spawnPointNames[i + 1] = bakedPoints[i].ID;
            }

            // Find current selection
            string currentSpawnID = _targetSpawnPointIDProp.stringValue;
            int currentSpawnIndex = 0;

            for (int i = 0; i < bakedPoints.Count; i++)
            {
                if (bakedPoints[i].ID == currentSpawnID)
                {
                    currentSpawnIndex = i + 1;
                    break;
                }
            }

            int newSpawnIndex = EditorGUILayout.Popup(
                "Target Spawn Point", currentSpawnIndex, spawnPointNames
            );

            if (newSpawnIndex != currentSpawnIndex)
            {
                _targetSpawnPointIDProp.stringValue = newSpawnIndex > 0 
                    ? bakedPoints[newSpawnIndex - 1].ID 
                    : "";
            }
        }

        /// <summary>
        /// Finds the index of the specified level in the loaded level data array.
        /// This method searches for a match within the `_levelDataArray` and returns an adjusted index
        /// to account for a "(None)" option in the level dropdown.
        /// </summary>
        /// <param name="level">The level to find within the `_levelDataArray`. Expected to be a <see cref="LevelData"/> object.</param>
        /// <returns>
        /// The 1-based index of the level in the `_levelDataArray`, or 0 if the level is null
        /// or not found.
        /// </returns>
        private int FindLevelIndex(LevelData level)
        {
            if (!level) return 0;

            for (int i = 0; i < _levelDataArray.Length; i++)
            {
                if (_levelDataArray[i] == level)
                    return i + 1;
            }

            return 0;
        }

        /// <summary>
        /// Caches the LevelRegistry and associated level data for use in the editor.
        /// This method retrieves LevelRegistry assets from the project's asset database and loads the first available registry.
        /// If no LevelRegistry assets are found, default values are used to indicate the absence of data.
        /// The method initializes and populates:
        /// - `_levelRegistry`: The loaded LevelRegistry asset.
        /// - `_levelDisplayNames`: An array of level display names for levels in the registry, including a "(None)" option.
        /// - `_levelDataArray`: An array of LevelData objects contained in the LevelRegistry.
        /// If no `LevelRegistry` exists or if loading fails, `_levelDisplayNames` is set to indicate that no registry was found,
        /// and `_levelDataArray` is initialized as an empty array.
        /// </summary>
        private void CacheLevelRegistry()
        {
            string[] guids = AssetDatabase.FindAssets("t:LevelRegistry");

            if (guids.Length == 0)
            {
                _levelRegistry = null;
                _levelDisplayNames = new string[] { "(No LevelRegistry found)" };
                _levelDataArray = Array.Empty<LevelData>();
                return;
            }

            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            _levelRegistry = AssetDatabase.LoadAssetAtPath<LevelRegistry>(path);

            if (_levelRegistry == null)
            {
                _levelDisplayNames = new string[] { "(No LevelRegistry found)" };
                _levelDataArray = Array.Empty<LevelData>();
                return;
            }

            IReadOnlyList<LevelData> levels = _levelRegistry.AllLevels;
            _levelDisplayNames = new string[levels.Count + 1];
            _levelDataArray = new LevelData[levels.Count];

            _levelDisplayNames[0] = "(None)";

            for (int i = 0; i < levels.Count; i++)
            {
                _levelDataArray[i] = levels[i];
                _levelDisplayNames[i + 1] = levels[i]
                    ? $"{levels[i].DisplayName} ({levels[i].SceneID})"
                    : "(Missing)";
            }
        }

        private void DrawValidation()
        {
            LevelTransitionTrigger trigger = (LevelTransitionTrigger)target;

            Collider2D collider = trigger.GetComponent<Collider2D>();

            if (collider && !collider.isTrigger)
            {
                EditorGUILayout.HelpBox(
                    "Collider2D must have 'Is Trigger' enabled for OnTriggerEnter2D to fire.",
                    MessageType.Error
                );
            }

            if (!trigger.TargetLevel)
            {
                EditorGUILayout.HelpBox(
                    "Target Level is not assigned. This trigger will not function.",
                    MessageType.Error
                );
            }

            if (string.IsNullOrEmpty(trigger.TargetSpawnPointID))
            {
                EditorGUILayout.HelpBox(
                    "Target Spawn Point ID is empty. Player positioning "
                    + "may fail after transition.",
                    MessageType.Warning
                );
            }
        }
    }
}
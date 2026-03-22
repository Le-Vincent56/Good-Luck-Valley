using System.Collections.Generic;
using GoodLuckValley.World.LevelManagement.Data;
using UnityEditor;

namespace GoodLuckValley.Editor.LevelManagement
{
    /// <summary>
    /// Custom inspector for <see cref="LevelData"/>. Validates scene ID,
    /// checks for duplicate or empty spawn point IDs, and warns about
    /// missing configuration.
    /// </summary>
    [CustomEditor(typeof(LevelData))]
    public class LevelDataEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            LevelData data = (LevelData)target;

            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Validation", EditorStyles.boldLabel);

            bool hasErrors = false;

            if (string.IsNullOrEmpty(data.SceneID))
            {
                EditorGUILayout.HelpBox(
                    "Scene ID is empty. This level cannot be loaded.",
                    MessageType.Error
                );
                
                hasErrors = true;
            }

            if (data.SpawnPointIDs.Count == 0)
            {
                EditorGUILayout.HelpBox(
                    "No spawn point IDs defined. Level transitions targeting "
                    + "this level may fail to find a spawn point.",
                    MessageType.Warning
                );
            }

            // Check for duplicate or empty spawn point IDs
            HashSet<string> seenIds = new HashSet<string>();
            IReadOnlyList<string> spawnPointIds = data.SpawnPointIDs;

            for (int i = 0; i < spawnPointIds.Count; i++)
            {
                string id = spawnPointIds[i];

                if (string.IsNullOrEmpty(id))
                {
                    EditorGUILayout.HelpBox(
                        $"Spawn point ID at index {i} is empty.",
                        MessageType.Error
                    );
                    
                    hasErrors = true;
                    continue;
                }

                if (!seenIds.Add(id))
                {
                    EditorGUILayout.HelpBox(
                        $"Duplicate spawn point ID '{id}' at index {i}.",
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
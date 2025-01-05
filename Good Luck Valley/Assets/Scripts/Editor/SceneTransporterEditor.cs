using GoodLuckValley.Scenes;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace GoodLuckValley.Editors.Scenes
{
    [CustomEditor(typeof(SceneTransporter))]
    public class SceneTransporterEditor : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            // Drwa the default inspector
            base.OnInspectorGUI();

            // Get the SceneTransporter component
            SceneTransporter sceneTransporter = (SceneTransporter)target;

            // Check if the SceneGroupData is not null and has a valid SceneGroup array
            if (sceneTransporter.SceneGroupData != null
                && sceneTransporter.SceneGroupData.SceneGroups != null && sceneTransporter.SceneGroupData.SceneGroups.Length > 0
            )
            {
                // Create an array to store the names of the Scenes
                string[] sceneGroupNames = new string[sceneTransporter.SceneGroupData.SceneGroups.Length];

                // Iterate through the SceneGroups array
                for (int i = 0; i < sceneTransporter.SceneGroupData.SceneGroups.Length; i++)
                {
                    // Store their names
                    sceneGroupNames[i] = sceneTransporter.SceneGroupData.SceneGroups[i].GroupName;
                }

                // Create a popup to select a Scene Group to load
                int selectedIndex = EditorGUILayout.Popup("Scene Group to Load", sceneTransporter.SceneIndexToLoad, sceneGroupNames);

                // Check if the selected index is different from the current Scene Index to Load
                if (selectedIndex != sceneTransporter.SceneIndexToLoad)
                {
                    // Add an Undo history
                    Undo.RecordObject(sceneTransporter, "Change Scene Group");

                    // Set the Scene Index to Load
                    SerializedObject serializedObject = new SerializedObject(sceneTransporter);
                    serializedObject.FindProperty("sceneIndexToLoad").intValue = selectedIndex;
                    serializedObject.ApplyModifiedProperties();

                    // Set the Editor as dirty
                    EditorUtility.SetDirty(sceneTransporter);
                }
            }
            else
            {
                // Display a helpbox to notify the user that no Scene Groups were found
                EditorGUILayout.HelpBox("No Scene Groups found. Please assign SceneGroups in the Inspector.", MessageType.Warning);
            }
        }
    }
}

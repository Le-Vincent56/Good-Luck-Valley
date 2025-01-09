using GoodLuckValley.Scenes;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

namespace GoodLuckValley.Editors.Scenes
{
    [CustomEditor(typeof(SceneGroupData))]
    public class SceneGroupDataEditor : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            SceneGroupData sceneGroupData = (SceneGroupData)target;

            // Check if the SceneLoader has a valid SceneGroups array
            if (sceneGroupData.SceneGroups != null && sceneGroupData.SceneGroups.Length > 0)
            {
                // Create an array of strings to hold the names of the SceneGroups
                string[] sceneGroupNames = new string[sceneGroupData.SceneGroups.Length];

                // Iterate through each element in SceneGroups
                for (int i = 0; i < sceneGroupData.SceneGroups.Length; i++)
                {
                    // Store the name of the SceneGroup in the array
                    sceneGroupNames[i] = sceneGroupData.SceneGroups[i].GroupName;
                }

                // Display the dropdown
                int selectedIndex = EditorGUILayout.Popup("Initial Group to Load", sceneGroupData.InitialScene, sceneGroupNames);

                // Check if the selected index is different from the SceneIndexToLoad
                if (selectedIndex != sceneGroupData.InitialScene)
                {
                    // Update the sceneIndexToLoad if the selection changes
                    Undo.RecordObject(sceneGroupData, "Change Scene Group");

                    // Update the serialized field
                    SerializedObject serializedObject = new SerializedObject(sceneGroupData);
                    serializedObject.FindProperty("InitialScene").intValue = selectedIndex;
                    serializedObject.ApplyModifiedProperties();

                    // Set the Editor as dirty
                    EditorUtility.SetDirty(sceneGroupData);
                }
            }
            else
            {
                // Otherwise, display a warning
                EditorGUILayout.HelpBox("No Scene Groups found. Please assign SceneGroups in the Inspector", MessageType.Warning);
            }

            base.OnInspectorGUI();
        }
    }
}

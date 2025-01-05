using GoodLuckValley.Scenes;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace GoodLuckValley.Editors.Scenes
{
    [CustomEditor(typeof(SceneTransporter))]
    public class SceneTransporterEditor : OdinEditor
    {
        public override void OnInspectorGUI()
        {
            // Drwa the default inspector
            base.OnInspectorGUI();

            SceneTransporter sceneTransporter = (SceneTransporter)target;

            if (sceneTransporter.SceneGroupData != null
                && sceneTransporter.SceneGroupData.SceneGroups != null && sceneTransporter.SceneGroupData.SceneGroups.Length > 0
            )
            {
                string[] sceneGroupNames = new string[sceneTransporter.SceneGroupData.SceneGroups.Length];
                for (int i = 0; i < sceneTransporter.SceneGroupData.SceneGroups.Length; i++)
                {
                    sceneGroupNames[i] = sceneTransporter.SceneGroupData.SceneGroups[i].GroupName;
                }

                int selectedIndex = EditorGUILayout.Popup("Scene Group to Load", sceneTransporter.SceneIndexToLoad, sceneGroupNames);

                if (selectedIndex != sceneTransporter.SceneIndexToLoad)
                {
                    Undo.RecordObject(sceneTransporter, "Change Scene Group");

                    SerializedObject serializedObject = new SerializedObject(sceneTransporter);
                    serializedObject.FindProperty("sceneIndexToLoad").intValue = selectedIndex;
                    serializedObject.ApplyModifiedProperties();

                    // Set the Editor as dirty
                    EditorUtility.SetDirty(sceneTransporter);
                }
            }
            else
            {
                EditorGUILayout.HelpBox("No Scene Groups found. Please assign SceneGroups in the Inspector.", MessageType.Warning);
            }
        }
    }
}

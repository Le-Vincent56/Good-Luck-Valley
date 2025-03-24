using GoodLuckValley.Cameras.Persistence;
using GoodLuckValley.Persistence;
using UnityEditor;
using UnityEngine;

namespace GoodLuckValley.Editors.Cameras
{
    [CustomEditor(typeof(CameraSaveHandler), true)]
    public class CameraSaveHandlerEditor : Editor
    {
        private CameraSaveHandler saveHandler;

        private void OnEnable()
        {
            saveHandler = (CameraSaveHandler)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Generate New SerializableGuid"))
            {
                saveHandler.ID = SerializableGuid.NewGuid();
                EditorUtility.SetDirty(saveHandler);
            }
        }
    }
}

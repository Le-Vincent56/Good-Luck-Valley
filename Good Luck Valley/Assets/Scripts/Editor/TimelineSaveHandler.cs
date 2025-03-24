using GoodLuckValley.Persistence;
using GoodLuckValley.World.Cinematics.Persistence;
using UnityEditor;
using UnityEngine;

namespace GoodLuckValley.Editors.Cameras
{
    [CustomEditor(typeof(TimelineSaveHandler), true)]
    public class TimelineSaveHandlerEditor : Editor
    {
        private TimelineSaveHandler saveHandler;

        private void OnEnable()
        {
            saveHandler = (TimelineSaveHandler)target;
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

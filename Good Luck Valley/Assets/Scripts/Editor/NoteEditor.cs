using GoodLuckValley.Persistence;
using UnityEditor;
using UnityEngine;

namespace GoodLuckValley.Journal.Collectibles.Editor
{
    [CustomEditor(typeof(Note))]
    public class NoteEditor : UnityEditor.Editor
    {
        Note note;

        private void OnEnable()
        {
            note = (Note)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if(GUILayout.Button("Generate New SerializableGuid"))
            {
                note.ID = SerializableGuid.NewGuid();
                EditorUtility.SetDirty(note);
            }
        }
    }
}
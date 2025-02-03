using GoodLuckValley.Interactables;
using GoodLuckValley.Persistence;
using UnityEditor;
using UnityEngine;

namespace GoodLuckValley.Editors.Interactables
{
    [CustomEditor(typeof(Collectible), true)]
    public class CollectibleEditor : Editor
    {
        Collectible collectible;

        private void OnEnable()
        {
            collectible = (Collectible)target;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            if (GUILayout.Button("Generate New SerializableGuid"))
            {
                collectible.ID = SerializableGuid.NewGuid();
                EditorUtility.SetDirty(collectible);
            }
        }
    }
}

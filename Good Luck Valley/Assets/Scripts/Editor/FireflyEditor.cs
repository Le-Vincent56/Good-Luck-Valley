using GoodLuckValley.Interactables.Fireflies;
using UnityEditor;
using UnityEngine;

namespace GoodLuckValley.Editors
{
    [CustomEditor(typeof(Firefly))]
    public class FireflyEditor : Editor
    {
        private Firefly firefly;

        private void OnEnable()
        {
            firefly = (Firefly)target;

            // Rnaodmize the values
            RandomizeValues();
        }

        public override void OnInspectorGUI()
        {
            // Draw the default inspector
            base.OnInspectorGUI();

            // Add a button to randomize values
            if (GUILayout.Button("Randomize Values"))
                // If the button is pressed, randomize the values for the Firefly
                RandomizeValues();

            // Check if the Inspector is updated
            if (GUI.changed)
                // If so, set dirty
                EditorUtility.SetDirty(firefly);
        }

        /// <summary>
        /// Randomize the values for each field
        /// </summary>
        private void RandomizeValues()
        {
            firefly.MaxSpeed = Random.Range(0.75f, 2f);
            firefly.PersonalSpace = Random.Range(0.4f, 0.75f);
            firefly.WanderTime = Random.Range(0.25f, 0.9f);
        }
    }
}

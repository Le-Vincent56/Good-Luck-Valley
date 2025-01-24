using GoodLuckValley.Interactables.Fireflies;
using UnityEditor;
using UnityEngine;

namespace GoodLuckValley.Editors
{
    [CustomEditor(typeof(Firefly))]
    public class FireflyEditor : Editor
    {
        // Define the ranges for randomization
        private float maxSpeedMin = 0.1f;
        private float maxSpeedMax = 0.5f;
        private float accelerationMin = 0.1f;
        private float accelerationMax = 3f;
        private float delecerationMin = 0.1f;
        private float delecerationMax = 0.5f;
        private float bounceSpeedMin = 0.1f;
        private float bounceSpeedMax = 0.5f;
        private float bounceAmplitudeMin = 0.0025f;
        private float bounceAmplitudeMax = 0.015f;

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
            //firefly.transform.position = new Vector3(
            //    firefly.transform.position.x, 
            //    firefly.transform.position.y, 
            //    Random.Range(-2f, 2f)
            //);
            firefly.MaxSpeed = Random.Range(maxSpeedMin, maxSpeedMax);
            firefly.Acceleration = Random.Range(accelerationMin, accelerationMax);
            firefly.DecelerationDistance = Random.Range(delecerationMin, delecerationMax);
            firefly.BounceSpeed = Random.Range(bounceSpeedMin, bounceSpeedMax);
            firefly.BounceAmplitude = Random.Range(bounceAmplitudeMin, bounceAmplitudeMax);
        }
    }
}

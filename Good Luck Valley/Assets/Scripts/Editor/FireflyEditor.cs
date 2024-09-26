using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace GoodLuckValley.Entities.Fireflies.Editor
{
    [CustomEditor(typeof(Firefly))]
    public class FireflyEditor : UnityEditor.Editor
    {
        // Define the ranges for randomization
        private float maxSpeedMin = 0.25f;
        private float maxSpeedMax = 5f;
        private float accelerationMin = 0.1f;
        private float accelerationMax = 3f;
        private float delecerationMin = 0.1f;
        private float delecerationMax = 0.5f;
        private float waitTimeMinMin = 0.5f;
        private float waitTimeMinMax = 1.5f;
        private float waitTimeMaxMin = 3f;
        private float waitTimeMaxMax = 5f;

        Firefly firefly;

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
            firefly.MaxSpeed = Random.Range(maxSpeedMin, maxSpeedMax);
            firefly.Acceleration = Random.Range(accelerationMin, accelerationMax);
            firefly.DecelerationDistance = Random.Range(delecerationMin, delecerationMax);
            firefly.WaitTimeMin = Random.Range(waitTimeMinMin, waitTimeMinMax);
            firefly.WaitTimeMax = Random.Range(waitTimeMaxMin, waitTimeMaxMax);
        }
    }
}
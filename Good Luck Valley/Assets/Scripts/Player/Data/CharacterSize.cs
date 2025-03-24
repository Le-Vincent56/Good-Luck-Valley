using System;
using UnityEditor;
using UnityEngine;

namespace GoodLuckValley.Player.Data
{
    [Serializable]
    public class CharacterSize
    {
        // Constant buffer values to prevent overlapping of colliders and ground checks
        public const float STEP_BUFFER = 0.05f;
        public const float COLLIDER_EDGE_RADIUS = 0.05f;

        // Height of the character, including collider and step height buffer
        [Range(0.1f, 10), Tooltip("How tall you are. This includes a collider and your step height.")]
        public float Height = 1.8f;

        // Width of the character's collider
        [Range(0.1f, 10), Tooltip("The width of your collider")]
        public float Width = 0.6f;

        // Height up to which the character can step over obstacles, like rocks or stairs
        [Range(STEP_BUFFER, 15), Tooltip("Step height allows you to step over rough terrain like steps and rocks.")]
        public float StepHeight = 0.5f;

        [Range(STEP_BUFFER, 15)]
        public float SlopeStepHeight = 0.16f;

        // Height of the character while crouching, defined as a percentage of the standing height
        [Range(0.1f, 10), Tooltip("A percentage of your height stat which determines your height while crouching. A smaller crouch requires more step height sacrifice")]
        public float CrouchHeight = 0.6f;

        // Buffer distance for ground check rays, preventing issues with slope navigation and drop edges
        [Range(0.01f, 0.2f), Tooltip("The outer buffer distance of the grounder rays. Reducing this too much can cause problems on slopes, too big and you can get stuck on the sides of drops.")]
        public float RayInset = 0.1f;

        [Range(0.0f, 0.2f), Tooltip("The offset to buffer the rays")]
        public float RayOffset = 0.0f;

        /// <summary>
        /// Generate the CharacterSize with validated and computed collider sizes and positions
        /// </summary>
        /// <returns></returns>
        public GeneratedCharacterSize GenerateCharacterSize()
        {
            // Ensures crouch and step heights adhere to valid boundaries
            ValidateHeights();

            // Create the GeneratedCharacterSize
            GeneratedCharacterSize characterSize = new GeneratedCharacterSize
            {
                Height = Height,
                Width = Width,
                StepHeight = StepHeight,
                RayInset = RayInset,
            };

            // Set the standing collider data
            characterSize.StandingColliderSize = new Vector2(characterSize.Width - COLLIDER_EDGE_RADIUS * 2, characterSize.Height - characterSize.StepHeight - COLLIDER_EDGE_RADIUS * 2);
            characterSize.StandingColliderCenter = new Vector2(0, characterSize.Height - characterSize.StandingColliderSize.y / 2 - COLLIDER_EDGE_RADIUS);

            // Set the crouching collider data
            characterSize.CrouchingHeight = CrouchHeight;
            characterSize.CrouchColliderSize = new Vector2(characterSize.Width - COLLIDER_EDGE_RADIUS * 2, characterSize.CrouchingHeight - characterSize.StepHeight - COLLIDER_EDGE_RADIUS * 2);
            characterSize.CrouchingColliderCenter = new Vector2(0, characterSize.CrouchingHeight - characterSize.CrouchColliderSize.y / 2 - COLLIDER_EDGE_RADIUS);

            return characterSize;
        }

        private static double _lastDebugLogTime;
        private const double TIME_BETWEEN_LOGS = 1f;

        /// <summary>
        ///  Validate the crouch and step height to make sure they stay within acceptable bounds
        ///  based on the character height
        /// </summary>
        private void ValidateHeights()
        {
#if UNITY_EDITOR
            // Enforce that the step height does not exceed character height minus a buffer
            float maxStepHeight = Height - STEP_BUFFER;
            if (StepHeight > maxStepHeight)
            {
                StepHeight = maxStepHeight;
                Log("Step height cannot be larger than height");
            }

            // Ensure that the crouch heigt is greater than the step height by at least a buffer amount
            float minCrouchHeight = StepHeight + STEP_BUFFER;
            if (CrouchHeight < minCrouchHeight)
            {
                CrouchHeight = minCrouchHeight;
                Log("Crouch height must be larger than step height");
            }

            // Logs a warning message in the Unity Editor, limited by a time buffer
            void Log(string text)
            {
                double time = EditorApplication.timeSinceStartup;
                if (_lastDebugLogTime + TIME_BETWEEN_LOGS > time) return;
                _lastDebugLogTime = time;
                Debug.LogWarning(text);
            }
#endif
        }
    }
}

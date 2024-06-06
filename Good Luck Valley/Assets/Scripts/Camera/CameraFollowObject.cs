using System.Collections;
using UnityEngine;

namespace GoodLuckValley.Cameras
{
    public class CameraFollowObject : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform playerTransform;

        [Header("Fields")]
        [SerializeField] private float flipYRotationTime = 0.5f;
        [SerializeField] private bool isFacingRight;
        private Coroutine turnCoroutine;

        void Update()
        {
            transform.position = playerTransform.position;
        }

        /// <summary>
        /// Set the player transform
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void OnSetPlayerTransform(Component sender, object data)
        {
            // Verify the correct data is being sent
            if (data is not Transform) return;

            // Cast the data
            Transform playerTransform = (Transform)data;

            // Set the data
            this.playerTransform = playerTransform;
        }

        /// <summary>
        /// Execute the directional bias turn
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void CallTurn(Component sender, object data)
        {
            // Verify the correct data is being sent
            if (data is not int) return;

            // Check if supposed to be panning
            if (CameraManager.Instance.IsPanning) return;

            // Cast the data
            int facingRight = (int)data;

            // If it's the same direction being sent, return to prevent directional
            // overlap
            bool isCheckRight = (facingRight == 1);
            if (isFacingRight == isCheckRight) return;

            // Start the turn coroutine
            turnCoroutine = StartCoroutine(FlipYLerp(facingRight));
        }

        /// <summary>
        /// Lerp the camera over the Y-direction for directional bias
        /// </summary>
        /// <param name="facingRight"></param>
        /// <returns></returns>
        private IEnumerator FlipYLerp(int facingRight)
        {
            // Get the start and end rotation
            float startRotation = transform.localEulerAngles.y;
            float endRotationAmount = DetermineEndRotation(facingRight);
            float yRotation;

            // Set a timer
            float elapsedTime = 0f;

            // Loop while the timer is less than the given rotation time
            while (elapsedTime < flipYRotationTime)
            {
                elapsedTime += Time.deltaTime;

                // Sine easing
                float t = elapsedTime / flipYRotationTime;
                t = Mathf.Sin(t * Mathf.PI * 0.5f);

                // Apply the lerp and set the rotation
                yRotation = Mathf.Lerp(startRotation, endRotationAmount, t);
                transform.rotation = Quaternion.Euler(0f, yRotation, 0f);

                yield return null;
            }

            // Ensure the final rotation is set
            transform.rotation = Quaternion.Euler(0f, endRotationAmount, 0f);
        }

        /// <summary>
        ///  Determine the rotation to end up at
        /// </summary>
        /// <param name="facingRight">Whether the player is facing right or not</param>
        /// <returns>A rotation angle in degrees to determine which way to face</returns>
        private float DetermineEndRotation(int facingRight)
        {
            // Set whether or not the player is facing right
            isFacingRight = (facingRight == 1);

            // Return an angle based on where the player is facing
            if (isFacingRight)
            {
                return 0f;
            }
            else
            {
                return 180f;
            }
        }
    }
}
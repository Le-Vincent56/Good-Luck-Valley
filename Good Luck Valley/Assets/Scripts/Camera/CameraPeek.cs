using GoodLuckValley.Player.Input;
using UnityEngine;

namespace GoodLuckValley.Cameras
{
    public class CameraPeek : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private InputReader input;

        [Header("Fields")]
        [SerializeField] private bool canPeek;
        [SerializeField] private bool inputPressed;
        [SerializeField] private float inputTime;
        [SerializeField] private bool peeking;
        [SerializeField] private float panDistance;
        [SerializeField] private float panTime;
        [SerializeField] private float peekYDamp;
        private float elapsedTime;

        private void OnEnable()
        {
            input.Look += OnLook;
        }

        private void OnDisable()
        {
            input.Look -= OnLook;
        }

        private void Update()
        {
            // Check if the player can peek at all
            if(!canPeek)
            {
                // Reset the peek variables
                ResetPeek();
                return;
            }

            // Check if already panning
            if (CameraManager.Instance.IsPanning)
            {
                // Reset the peek variables
                ResetPeek();
                return;
            }
            

            // Check if the camera is the default, follow camera
            if (!CameraManager.Instance.IsDefaultCam)
            {
                // Reset the peek variables
                ResetPeek();
                return;
            }

            // Check if the input is being pressed
            if(inputPressed)
            {
                // Check if the elapsed time is less than the input time
                if (elapsedTime < inputTime)
                {
                    // Increment the time
                    elapsedTime += Time.deltaTime;
                }
                // Check if the elapsed time is greater or equal to the input time and the camera currently is not panned
                else if (elapsedTime >= inputTime && !peeking)
                {
                    // Pan the camera
                    CameraManager.Instance.Peek(panDistance, panTime, new Vector2(0f, input.NormLookY), peekYDamp, false);
                    peeking = true;
                }
            } else if(!inputPressed && peeking) // Only execute if already peeking
            {
                // Pan the camera back
                CameraManager.Instance.Peek(panDistance, panTime, new Vector2(0f, input.NormLookY), peekYDamp, true);

                // Reset the peek variables
                ResetPeek();
            }
        }

        /// <summary>
        /// Handle Look input
        /// </summary>
        /// <param name="inputPressed"></param>
        private void OnLook(bool inputPressed)
        {
            // Set input pressed
            this.inputPressed = inputPressed;
        }

        /// <summary>
        /// Reset peek variables
        /// </summary>
        private void ResetPeek()
        {
            inputPressed = false;
            peeking = false;
            elapsedTime = 0f;
        }

        /// <summary>
        /// Set whether or not the player can peek
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void SetCanPeek(Component sender, object data)
        {
            // Verify that the correct data was sent
            if (data is not bool) return;

            // Cast the data
            bool canPeek = (bool)data;

            // Set the data
            this.canPeek = canPeek;
        }
    }
}
using GoodLuckValley.Patterns.Singletons;
using UnityEngine;
using Cinemachine;
using System.Collections;

namespace GoodLuckValley.Cameras
{
    public class CameraManager : Singleton<CameraManager>
    {
        [Header("References")]
        [SerializeField] private CinemachineVirtualCamera[] virtualCameras;
        [SerializeField] private CinemachineVirtualCamera activeCamera;

        [Header("Fields")]
        [SerializeField] private float fallPanAmount = 2f;
        [SerializeField] private float fallDampAmount;
        [SerializeField] private float fallPanDownTime = 0.35f;
        [SerializeField] private float fallPanReturnTime = 0.15f;
        [SerializeField] private float fallSpeedDampingChangeThreshold = -4;
        [SerializeField] private float normYPanAmount;
        [SerializeField] private float normXPanAmount;
        private Vector2 startingTrackedObjectOffset;
        private Coroutine lerpYPanCoroutine;
        private Coroutine panCameraCoroutine;
        private Coroutine peekCameraCoroutine;
        private CinemachineFramingTransposer framingTransposer;

        public bool IsDefaultCam => activeCamera == virtualCameras[0];
        public bool IsPanning { get; private set; }
        public bool IsLerpingYDamping { get; private set; }
        public bool LerpedFromPlayerFalling { get; set; }
        public float FallSpeedDampingChangeThreshold { get { return fallSpeedDampingChangeThreshold; } }

        protected override void Awake()
        {
            // Initialize the singleton
            base.Awake();

            // Loop through the virtual cameras
            for(int i = 0; i < virtualCameras.Length; i++)
            {
                // Check if the camera is enabled
                if (virtualCameras[i].enabled)
                {
                    // Set it to the active camera
                    activeCamera = virtualCameras[i];

                    // Set the framing transposer
                    framingTransposer = activeCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                }
            }

            // Set the y-damping amount
            normYPanAmount = framingTransposer.m_YDamping;
            normXPanAmount = framingTransposer.m_XDamping;

            // Set the starting position of the tracked object offset
            startingTrackedObjectOffset = framingTransposer.m_TrackedObjectOffset;
        }

        public void LerpYDamping(bool isPlayerFalling)
        {
            if(lerpYPanCoroutine != null)
                StopCoroutine(lerpYPanCoroutine);

            lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
        }

        private IEnumerator LerpYAction(bool isPlayerFalling)
        {
            IsLerpingYDamping = true;

            Vector2 endPos;
            Vector2 startPos;

            float panTime = fallPanDownTime;

            // Determine the end damping amount
            if(isPlayerFalling)
            {
                LerpedFromPlayerFalling = true;

                // Set the framing transposer's damping
                framingTransposer.m_YDamping = fallDampAmount;

                // Get the end position
                endPos = new Vector2(0, -fallPanAmount);
                startPos = startingTrackedObjectOffset;

                // Add the positions together
                endPos += startPos;
            } else
            {
                panTime = fallPanReturnTime;

                // Set the framing transposer's damping
                framingTransposer.m_YDamping = 0;

                // Set the starting position to the current tracked object offset
                startPos = framingTransposer.m_TrackedObjectOffset;

                // Set the end position to the original tracked object offset
                endPos = startingTrackedObjectOffset;
            }

            // Lerp the pan amount
            float elapsedTime = 0f;
            while(elapsedTime < panTime)
            {
                elapsedTime += Time.deltaTime;

                // Calculate t for easing
                float t = elapsedTime / panTime;
                t = Mathf.SmoothStep(0f, 1f, t);

                // Lerp the pan
                Vector3 panLerp = Vector3.Lerp(startPos, endPos, t);

                // Set the pan
                framingTransposer.m_TrackedObjectOffset = panLerp;

                framingTransposer.m_YDamping = normYPanAmount;

                yield return null;
            }

            IsLerpingYDamping = false;
        }

        public void Peek(float peekDistance, float panTime, Vector2 peekDirection, Vector2 peekDamp, bool panToStartingPos)
        {
            peekCameraCoroutine = StartCoroutine(PeekCamera(peekDistance, panTime, peekDirection, peekDamp, panToStartingPos));
        }

        private IEnumerator PeekCamera(float peekDistance, float panTime, Vector2 peekDirection, Vector2 peekDamp, bool panToStartingPos)
        {
            // Establish a starting and end position
            Vector2 endPos;
            Vector2 startingPos;

            // Handle pan from trigger
            if (!panToStartingPos)
            {
                // Set the framing transposer's damping
                framingTransposer.m_YDamping = peekDamp.y;
                framingTransposer.m_XDamping = peekDamp.x;

                // Get the peek direction
                endPos = peekDirection * peekDistance;

                // Set the starting position
                startingPos = startingTrackedObjectOffset;

                // Add the starting position to the end position
                endPos += startingPos;
            }
            else // Handle a pan back to the starting position
            {
                // Set the starting position to the current tracked object offset
                startingPos = framingTransposer.m_TrackedObjectOffset;

                // Set the end position to the original tracked object offset
                endPos = startingTrackedObjectOffset;
            }

            // Handle the panning
            float elapsedTime = 0f;
            while (elapsedTime < panTime)
            {
                elapsedTime += Time.deltaTime;

                // Calculate t for easing
                float t = elapsedTime / panTime;
                t = Mathf.SmoothStep(0f, 1f, t);

                // Lerp the pan
                Vector3 panLerp = Vector3.Lerp(startingPos, endPos, t);

                // Set the pan
                framingTransposer.m_TrackedObjectOffset = panLerp;

                yield return null;
            }

            framingTransposer.m_YDamping = normYPanAmount;
            framingTransposer.m_XDamping = normXPanAmount;
        }

        public void PanCameraOnContact(float panDistance, float panTime, Vector2 panDirection, bool panToStartingPos)
        {
            panCameraCoroutine = StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStartingPos));
        }

        private IEnumerator PanCamera(float panDistance, float panTime, Vector2 panDirection, bool panToStartingPos)
        {
            // Establish a starting and end position
            Vector2 endPos;
            Vector2 startingPos;

            // Handle pan from trigger
            if(!panToStartingPos)
            {
                // Normalize the pan direction
                endPos = new Vector2(Mathf.Abs(panDirection.x), panDirection.y);

                // Set the pan distance
                endPos *= panDistance;

                // Set the starting position
                startingPos = startingTrackedObjectOffset;

                // Add the starting position to the end position
                endPos += startingPos;
            } else // Handle a pan back to the starting position
            {
                // Set the starting position to the current tracked object offset
                startingPos = framingTransposer.m_TrackedObjectOffset;

                // Set the end position to the original tracked object offset
                endPos = startingTrackedObjectOffset;
            }

            // Handle the panning
            float elapsedTime = 0f;
            while(elapsedTime < panTime)
            {
                elapsedTime += Time.deltaTime;

                // Lerp the pan
                Vector3 panLerp = Vector3.Lerp(startingPos, endPos, (elapsedTime / panTime));

                // Set the pan
                framingTransposer.m_TrackedObjectOffset = panLerp;

                yield return null;
            }

            // If exiting, then set panning to false
            IsPanning = !panToStartingPos;
        }

        public void SwapCamera(CinemachineVirtualCamera cameraFromLeft, CinemachineVirtualCamera cameraFromRight, Vector2 triggerExitDirection)
        {
            // Check if the current camera is the camera on the left and the exit direction was on the right
            if(activeCamera == cameraFromLeft && triggerExitDirection.x > 0f)
            {
                SwitchCamera(cameraFromLeft, cameraFromRight);
            } 
            // Check if the current camera is the camera on the right and the trigger exit direction was on the left
            else if(activeCamera == cameraFromRight && triggerExitDirection.x < 0f)
            {
                SwitchCamera(cameraFromRight, cameraFromLeft);
            }
        }

        private void SwitchCamera(CinemachineVirtualCamera oldCamera, CinemachineVirtualCamera newCamera)
        {
            // Activate the new camera
            newCamera.enabled = true;

            // Deactivate the old camera
            oldCamera.enabled = false;

            // Set the new camera as the current camera
            activeCamera = newCamera;

            // Update the framing composer
            framingTransposer = activeCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
    }
}
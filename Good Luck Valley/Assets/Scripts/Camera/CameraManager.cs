using GoodLuckValley.Patterns.Singletons;
using UnityEngine;
using Cinemachine;
using System.Collections;
using GoodLuckValley.Events;
using UnityEngine.UIElements;

namespace GoodLuckValley.Cameras
{
    public enum CameraType
    {
        Default = 0,
        Peek = 1,
        Tight = 2,
        Showcase = 3,
        Locked = 4,
    }

    public class CameraManager : Singleton<CameraManager>
    {
        [Header("Events")]
        [SerializeField] private GameEvent onUpdateScreenBounds;

        [Header("References")]
        [SerializeField] private CinemachineVirtualCamera[] virtualCameras;
        [SerializeField] private CinemachineVirtualCamera activeCamera;

        [Header("Fields - General")]
        [SerializeField] private float normYPanAmount;
        [SerializeField] private float normXPanAmount;
        [SerializeField] private bool peeking;

        [Header("Fields - Falling")]
        [SerializeField] private float fallPanAmount = 2f;
        [SerializeField] private float fallDampAmount;
        [SerializeField] private float fallPanDownTime = 0.35f;
        [SerializeField] private float fallPanReturnTime = 0.15f;
        [SerializeField] private float fallSpeedDampingChangeThreshold = -4;

        [Header("Fields - Sliding")]
        [SerializeField] private Vector2 slidePanAmount;
        [SerializeField] private Vector2 slideDampAmount;
        [SerializeField] private float slideFollowLerpTime = 1.8f;
        [SerializeField] private float slidePanDownTime = 0.15f;
        [SerializeField] private float slidePanReturnTime = 0.15f;

        private Vector2 startingTrackedObjectOffset;
        private Vector2 currentTrackedObjectOffset;
        private Vector2 targetTrackedObjectOffset;
        private Coroutine lerpFallOffsetCoroutine;
        private Coroutine lerpSlideOffsetCoroutine;
        private Coroutine panCameraCoroutine;
        private Coroutine peekCameraCoroutine;
        private CinemachineFramingTransposer framingTransposer;

        public bool IsDefaultCam => activeCamera == virtualCameras[(int)CameraType.Default];
        public bool IsPeekCam => activeCamera == virtualCameras[(int)CameraType.Peek];
        public bool IsPanning { get; private set; }
        public bool IsLerpingFallOffset { get; private set; }
        public bool LerpedFromPlayerFalling { get; set; }
        public bool IsLerpingSlideOffset { get; private set; }
        public bool LerpedFromPlayerSliding { get; set; }
        public bool LerpingOffsets { get => IsLerpingFallOffset || IsLerpingSlideOffset; }
        public float FallSpeedDampingChangeThreshold { get { return fallSpeedDampingChangeThreshold; } }

        protected override void Awake()
        {
            // Initialize the singleton
            base.Awake();

            // Loop through the virtual cameras
            for (int i = 0; i < virtualCameras.Length; i++)
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
            currentTrackedObjectOffset = startingTrackedObjectOffset;
            targetTrackedObjectOffset = startingTrackedObjectOffset;
        }

        private void Update()
        {
            Bounds bounds = CalculateCameraBounds();

            onUpdateScreenBounds.Raise(this, bounds);
        }

        private Bounds CalculateCameraBounds()
        {
            float height = 2f * Camera.main.orthographicSize;
            float width = height * Camera.main.aspect;

            Vector3 center = Camera.main.transform.position;
            Vector3 size = new Vector3(width, height);

            return new Bounds(center, size);
        }

        public void SetTrackedOffset(Vector2 pos) => framingTransposer.m_TrackedObjectOffset = pos;

        #region PEEKING
        public void Peek(Vector2 offset, Vector2 peekDamping, float peekDistance, float peekLerp)
        {
            Vector2 newTargetPosition = currentTrackedObjectOffset + offset;
            Vector2 clampedTargetPosition = Vector2.ClampMagnitude(newTargetPosition - startingTrackedObjectOffset, peekDistance) + startingTrackedObjectOffset;

            targetTrackedObjectOffset = clampedTargetPosition;

            // Set damping
            framingTransposer.m_XDamping = peekDamping.x;
            framingTransposer.m_YDamping = peekDamping.y;

            if(peekCameraCoroutine == null)
                peekCameraCoroutine = StartCoroutine(PeekCamera(peekLerp));

            peeking = true;
        }

        public void Unpeek()
        {
            currentTrackedObjectOffset = startingTrackedObjectOffset;
            targetTrackedObjectOffset = startingTrackedObjectOffset;

            peeking = false;
        }

        private IEnumerator PeekCamera(float peekLerp)
        {
            while (true)
            {
                // Calculate the new position based on the target position
                currentTrackedObjectOffset = Vector2.Lerp(currentTrackedObjectOffset, targetTrackedObjectOffset, peekLerp);

                framingTransposer.m_TrackedObjectOffset = currentTrackedObjectOffset;

                if (Vector2.Distance(currentTrackedObjectOffset, targetTrackedObjectOffset) < 0.1f)
                {
                    framingTransposer.m_TrackedObjectOffset = targetTrackedObjectOffset;
                    currentTrackedObjectOffset = targetTrackedObjectOffset;
                    peekCameraCoroutine = null;

                    framingTransposer.m_XDamping = normXPanAmount;
                    framingTransposer.m_YDamping = normYPanAmount;

                    yield break;
                }

                yield return null;
            }
        }
        #endregion

        #region FALL-LERPING
        public void LerpFallOffset(bool isPlayerFalling)
        {
            if(lerpFallOffsetCoroutine != null)
                StopCoroutine(lerpFallOffsetCoroutine);

            lerpFallOffsetCoroutine = StartCoroutine(LerpFallAction(isPlayerFalling));
        }

        private IEnumerator LerpFallAction(bool isPlayerFalling)
        {
            IsLerpingFallOffset = true;

            Vector2 endPos;
            Vector2 startPos;

            float panTime = fallPanDownTime;

            // Determine the end damping amount
            if(isPlayerFalling)
            {
                LerpedFromPlayerFalling = true;

                // Set the framing transposer's damping
                framingTransposer.m_YDamping = fallDampAmount;

                // Get the start and end position
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

                // Reset damping
                framingTransposer.m_YDamping = normYPanAmount;

                yield return null;
            }

            IsLerpingFallOffset = false;
        }
        #endregion

        #region SLIDE-LERPING
        public void LerpSlopeOffset(CameraFollowObject followObject, Vector2 slideDirection, bool isPlayerSliding, bool changedDirections = false)
        {
            // If peeking, unpeek
            if (peeking)
                Unpeek();

            if (lerpSlideOffsetCoroutine != null)
                StopCoroutine(lerpSlideOffsetCoroutine);

            // Check if the player is changing directions
            if(changedDirections && isPlayerSliding)
            {
                followObject.SetLerpTime(slideFollowLerpTime);

                lerpSlideOffsetCoroutine = StartCoroutine(LerpSlopeActionY(slideDirection.y));
            } else
            {
                // Set the default lerp time for the follow obejct
                followObject.SetLerpTime(followObject.DefaultLerpTime);

                lerpSlideOffsetCoroutine = StartCoroutine(LerpSlopeAction(slideDirection, isPlayerSliding));
            }
        }

        private IEnumerator LerpSlopeActionY(float slideDirectionY)
        {
            IsLerpingSlideOffset = true;
            LerpedFromPlayerSliding = true;

            Vector2 startPos;
            Vector2 endPos;

            // Set the framing transposer's damping
            framingTransposer.m_XDamping = slideDampAmount.x;
            framingTransposer.m_YDamping = slideDampAmount.y;

            // Get the start and end position
            float slidePanY = (Mathf.Sign(slideDirectionY) < 0) ? -slidePanAmount.y : slidePanAmount.y;

            endPos = new Vector2(0, slidePanY * 2);
            startPos = framingTransposer.m_TrackedObjectOffset;

            // Combine the positions
            endPos += startPos;

            endPos.x = startingTrackedObjectOffset.x + slidePanAmount.x;
            endPos.y = Mathf.Clamp(endPos.y, -2, 2);

            // Lerp the pan amount
            float elapsedTime = 0f;
            while (elapsedTime < slidePanDownTime)
            {
                elapsedTime += Time.deltaTime;

                // Sine easing
                float t = elapsedTime / slidePanDownTime;
                t = Mathf.Sin(t * Mathf.PI * 0.5f);

                // Lerp the pan
                Vector3 panLerp = Vector3.Lerp(startPos, endPos, t);

                // Set the pan
                framingTransposer.m_TrackedObjectOffset = panLerp;

                // Reset damping
                framingTransposer.m_YDamping = 1;

                yield return null;
            }

            framingTransposer.m_TrackedObjectOffset = endPos;

            IsLerpingSlideOffset = false;
        }

        private IEnumerator LerpSlopeAction(Vector2 slideDirection,bool isPlayerSliding)
        {
            IsLerpingSlideOffset = true;

            Vector2 startPos;
            Vector2 endPos;

            float panTime = slidePanDownTime;

            if(isPlayerSliding)
            {
                LerpedFromPlayerSliding = true;

                // Set the framing transposer's damping
                framingTransposer.m_XDamping = slideDampAmount.x;
                framingTransposer.m_YDamping = slideDampAmount.y;

                // Get the start and end position
                float slidePanY = (Mathf.Sign(slideDirection.y) < 0) ? -slidePanAmount.y : slidePanAmount.y;

                endPos = new Vector2(slidePanAmount.x, slidePanY);
                startPos = framingTransposer.m_TrackedObjectOffset;

                // Combine the positions
                endPos += startPos;

                endPos.x = startingTrackedObjectOffset.x + slidePanAmount.x;
                endPos.y = Mathf.Clamp(endPos.y, -2, 2);
            } else
            {
                // Set the pan time to the returning time
                panTime = slidePanReturnTime;

                // Set the framing transposer's damping
                framingTransposer.m_XDamping = normXPanAmount;
                framingTransposer.m_YDamping = normYPanAmount;

                // Set the starting position to the current tracked object offset
                startPos = framingTransposer.m_TrackedObjectOffset;

                // Set the end position to the original tracked object offset
                endPos = startingTrackedObjectOffset;
            }

            // Lerp the pan amount
            float elapsedTime = 0f;
            while (elapsedTime < panTime)
            {
                elapsedTime += Time.deltaTime;

                // Sine easing
                float t = elapsedTime / panTime;
                t = Mathf.Sin(t * Mathf.PI * 0.5f);

                // Lerp the pan
                Vector3 panLerp = Vector3.Lerp(startPos, endPos, t);

                // Set the pan
                framingTransposer.m_TrackedObjectOffset = panLerp;

                // Reset damping
                framingTransposer.m_XDamping = normXPanAmount;
                framingTransposer.m_YDamping = normYPanAmount;

                yield return null;
            }

            framingTransposer.m_TrackedObjectOffset = endPos;

            IsLerpingSlideOffset = false;
        }
        #endregion

        #region PANNING
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
        #endregion

        #region CAMERA SWITCHING
        public void SwapCamera(CinemachineVirtualCamera cameraFromLeft, CinemachineVirtualCamera cameraFromRight, Vector2 triggerExitDirection, bool horizontal = true)
        {
            
            if(horizontal)
            {
                // Check if the current camera is the camera on the left and the exit direction was on the right
                if (activeCamera == cameraFromLeft && triggerExitDirection.x > 0f)
                {
                    SwitchCamera(cameraFromLeft, cameraFromRight);
                }
                // Check if the current camera is the camera on the right and the trigger exit direction was on the left
                else if (activeCamera == cameraFromRight && triggerExitDirection.x < 0f)
                {
                    SwitchCamera(cameraFromRight, cameraFromLeft);
                }
            } else
            {
                // Check if the current camera is the camera on the right and the trigger exit direction was on the left
                if (activeCamera == cameraFromLeft && triggerExitDirection.y < 0f)
                {
                    SwitchCamera(cameraFromLeft, cameraFromRight);
                }
                // Check if the current camera is the camera on the right and the trigger exit direction was on the left
                else if (activeCamera == cameraFromRight && triggerExitDirection.y > 0f)
                {
                    SwitchCamera(cameraFromRight, cameraFromLeft);
                }
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

        public void SwitchCamera(CameraType newCamera)
        {
            activeCamera.enabled = false;

            activeCamera = virtualCameras[(int)newCamera];

            activeCamera.enabled = true;

            framingTransposer = activeCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
        #endregion
    }
}
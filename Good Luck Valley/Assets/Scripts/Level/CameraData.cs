using Cinemachine;
using GoodLuckValley.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Level
{
    public class CameraData : MonoBehaviour
    {
        public struct ScreenBounds
        {
            public float Left;
            public float Right;
            public float Bottom;
            public float Top;
            public ScreenBounds(float leftBound, float rightBound, float bottomBound, float topBound)
            {
                Left = leftBound;
                Right = rightBound;
                Bottom = bottomBound;
                Top = topBound;
            }
        }

        #region REFERENCES
        [Header("Events")]
        [SerializeField] private GameEvent onCameraMove;
        [SerializeField] private GameEvent onUpdateCursorBounds;

        [SerializeField] private CinemachineVirtualCamera smartCam;
        #endregion

        #region FIELDS
        [SerializeField] private Rect currentMainViewport;
        [SerializeField] private Rect previousCMViewport;
        [SerializeField] private Rect currentCMViewport;
        [SerializeField] private ScreenBounds currentBounds;
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            smartCam = GetComponent<CinemachineVirtualCamera>();

            // Set initial viewports
            currentMainViewport = CalcViewportRectMain();
            previousCMViewport = CalcViewportRectCM();
            currentCMViewport = previousCMViewport;

            // Initial bounds
            currentBounds = new ScreenBounds(
                currentCMViewport.xMin,
                currentCMViewport.xMax,
                currentCMViewport.yMin,
                currentCMViewport.yMax
            );
        }

        // Update is called once per frame
        void Update()
        {
            // Update the current viewport
            currentMainViewport = CalcViewportRectMain();
            currentCMViewport = CalcViewportRectCM();

            // Check if it is worth updating the viewports
            if (DiffCMViewports())
            {
                // Calculate width offset and set bounds
                float widthOffset = currentCMViewport.x - currentMainViewport.x;
                currentBounds.Left = currentCMViewport.xMin - widthOffset;
                currentBounds.Right = currentCMViewport.xMax - widthOffset;

                // Calculate height offset and set bounds
                float heightOffset = currentCMViewport.y - currentMainViewport.y;
                currentBounds.Bottom = currentCMViewport.yMin - heightOffset;
                currentBounds.Top = currentCMViewport.yMax - heightOffset;

                // Raise events on camera move
                //  - Calls to: TODO: PARALLAX
                onCameraMove.Raise(this, true);
            }
        }

        /// <summary>
        /// Compare the current and previous Cinemachine viewports
        /// </summary>
        /// <returns>Returns true if there's a worthy difference, false if not</returns>
        private bool DiffCMViewports()
        {
            // Check if there's a big enough difference to warrant a change
            if (Mathf.Abs(currentCMViewport.x - previousCMViewport.x) > 0.001f) return true;
            if (Mathf.Abs(currentCMViewport.y - previousCMViewport.y) > 0.001f) return true;

            // Return false if all cases pass through
            return false;
        }

        /// <summary>
        /// Calculate the viewport rectangle of the Cinemachine camera
        /// </summary>
        /// <returns>The viewport rectangle of the Cinemachine camera</returns>
        private Rect CalcViewportRectCM()
        {
            // Set orthographic size and aspect ratio of the Cinemachine
            // Camera
            float orthographicSize = smartCam.m_Lens.OrthographicSize;
            float aspectRatio = smartCam.m_Lens.Aspect;

            // Set viewport dimensions
            float viewportHeight = orthographicSize * 2;
            float viewportWidth = viewportHeight * aspectRatio;

            // Get the current camera position
            Vector2 cameraPosition = smartCam.transform.position;

            // Create a viewport rectangle
            Rect viewportRect = new Rect(
                cameraPosition.x - viewportWidth / 2,
                cameraPosition.y - viewportHeight / 2,
                viewportWidth,
                viewportHeight);

            return viewportRect;
        }

        /// <summary>
        /// Calculate the viewport rectangle of the Main Camera
        /// </summary>
        /// <returns>The viewport rectangle of the Main Camera</returns>
        private Rect CalcViewportRectMain()
        {
            // Get the orthographic size and aspect ratio of the Main Camera
            float orthographicSize = Camera.main.orthographicSize;
            float aspectRatio = Camera.main.aspect;

            // Set viewport dimensions
            float camHeight = orthographicSize * 2;
            float camWidth = camHeight * aspectRatio;

            // Create a viewport rectangle
            Rect viewportRect = new Rect(
                Camera.main.transform.position.x - camWidth / 2,
                Camera.main.transform.position.y - camHeight / 2,
                camWidth,
                camHeight);

            return viewportRect;
        }

        public void OnGetCursorBounds(Component sender, object data)
        {
            // Send the current bounds out
            // Calls to:
            //  - GameCursor.UpdateCursorBounds();
            onUpdateCursorBounds.Raise(this, currentBounds);
        }
    }
}
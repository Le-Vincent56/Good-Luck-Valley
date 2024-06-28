using GoodLuckValley.Events;
using GoodLuckValley.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GoodLuckValley.Cameras
{
    public class CameraOmniPeek : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private GameEvent onLearnPeek;

        [Header("Fields - Boundaries")]
        [SerializeField] private Vector2 boundaryPadding;
        [SerializeField] private Bounds currentBounds;

        [Header("Fields - Input")]
        [SerializeField] private float tryPeekBuffer;
        [SerializeField] private float tryPeekTimeMax;
        [SerializeField] private bool tryingToPeek;

        [Header("Fields - Peek")]
        [SerializeField] private float peekDistance;
        [SerializeField] private float peekLerp;
        [SerializeField] private Vector2 peekDamp;
        [SerializeField] private bool peeking;
        [SerializeField] private bool canPeek;

        [Header("Fields - Safety")]
        [SerializeField] private float safetyTime;
        [SerializeField] private float safetyTimeMax;

        [Header("Fields - Positions")]
        [SerializeField] private float currentDistance;
        [SerializeField] private Vector2 currentFollowPos;
        [SerializeField] private Vector2 calculatedFollowPos;
        [SerializeField] private Vector2 cursorPos;
        [SerializeField] private Vector2 currentDirection;
        [SerializeField] private Vector2 calculatedDirection;
        private Rect noPeekZone;

        private void Update()
        {
            // Check if already panning
            if (CameraManager.Instance.IsPanning)
            {
                // Reset the peek variables
                return;
            }

            // Check if the camera is the default follow camera or the peek camera
            if (!CameraManager.Instance.IsDefaultCam && !CameraManager.Instance.IsPeekCam)
            {
                // Reset the peek variables
                return;
            }

            if (peeking && !canPeek)
            {
                ResetPeek();
            }

            if (!canPeek) return;

            // Create rect if not created already
            if (noPeekZone == null)
                noPeekZone = new Rect();

            // Set bounds with added padding
            noPeekZone.xMin = currentBounds.min.x + boundaryPadding.x;
            noPeekZone.xMax = currentBounds.max.x - boundaryPadding.x;
            noPeekZone.yMax = currentBounds.max.y - boundaryPadding.y;
            noPeekZone.yMin = currentBounds.min.y + boundaryPadding.y;

            // Get cursor position
            Vector2 rawCursorPos = Camera.main.ScreenToWorldPoint(
                new Vector2(
                    Mouse.current.position.ReadValue().x,
                    Mouse.current.position.ReadValue().y
            )
            );

            cursorPos = Vector2Extensions.Round(rawCursorPos, 1);

            if (!peeking && safetyTime > 0)
                safetyTime -= Time.deltaTime;

            // Check if the cursor is within the bounds of the noPeekZone
            if (cursorPos.x < noPeekZone.xMin || cursorPos.x > noPeekZone.xMax ||
                cursorPos.y < noPeekZone.yMin || cursorPos.y > noPeekZone.yMax)
            {
                if(safetyTime <= 0f)
                {
                    // If so, try to peek
                    tryingToPeek = true;
                }
            }
            else
            {
                // If peeking, reset the peek
                if(peeking)
                    ResetPeek();
            }

            // Check if we are trying to peek, but have not yet peeked
            if (tryingToPeek && !peeking)
            {
                // Increment the peek buffer
                tryPeekBuffer += Time.deltaTime;

                // Check if the player has been trying to peek for long enough
                if (tryPeekBuffer >= tryPeekTimeMax)
                {
                    // Learn peeking
                    onLearnPeek.Raise(this, null);

                    // Set peeking to true
                    peeking = true;

                    CameraManager.Instance.SwitchCamera(CameraType.Peek);
                }
            }

            // Check if the camera is peeking
            if (peeking)
            {
                // Get the current direction to the cursor from the follow object
                currentDirection = (cursorPos - currentFollowPos).normalized;

                // Get the calculated destination position
                calculatedFollowPos = Vector2Extensions.Round(currentFollowPos + (currentDirection * peekDistance), 1);

                // Get the direction to the calculated follow position
                calculatedDirection = calculatedFollowPos - currentFollowPos;

                // Peek towards that direction
                CameraManager.Instance.Peek(calculatedDirection.normalized, peekDamp, peekDistance, peekLerp);
            }
        }

        /// <summary>
        /// Move the camera back to the default position and reset peek variables
        /// </summary>
        private void ResetPeek()
        {
            // Reset camera
            CameraManager.Instance.SwitchCamera(CameraType.Default);

            // If not, then stop trying to peek
            tryingToPeek = false;

            // Check if peekiing
            if (peeking)
            {
                // Stop peeking
                CameraManager.Instance.Unpeek();
            }

            // Reset peeking variables
            tryPeekBuffer = 0f;
            peeking = false;
            safetyTime = safetyTimeMax;
        }

        /// <summary>
        /// Update the current camera bounds
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void UpdateCurrentBounds(Component sender, object data)
        {
            // Make sure the correct data was sent
            if (data is not Bounds) return;

            // Cast and update data
            currentBounds = (Bounds)data;
        }

        /// <summary>
        /// Update the position of the follow object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void UpdateFollowPosition(Component sender, object data)
        {
            // Make sure the correct data was sent
            if (data is not Vector2) return;

            // Cast and update data
            currentFollowPos = Vector2Extensions.Round((Vector2)data, 1);
        }

        /// <summary>
        /// Set whether or not the player can peek
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void SetCanPeek(Component sender, object data)
        {
            // Verify that the correct data is sent
            if (data is not bool) return;

            // Cast and update data
            canPeek = (bool)data;
        }

        private void OnDrawGizmos()
        {
            if (noPeekZone != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(noPeekZone.center, noPeekZone.size);
            }

            if (calculatedFollowPos != null && currentFollowPos != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(currentFollowPos, calculatedFollowPos);
            }
        }
    }
}
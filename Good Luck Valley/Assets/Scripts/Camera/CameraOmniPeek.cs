using GoodLuckValley.Cameras;
using GoodLuckValley.Events;
using GoodLuckValley.Extensions;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraOmniPeek : MonoBehaviour
{
    [Header("Events")]
    [SerializeField] private GameEvent onLearnPeek;

    [Header("Fields - Boundaries")]
    [SerializeField] private float boundaryPadding;
    [SerializeField] private CameraData.ScreenBounds currentBounds;

    [Header("Fields - Input")]
    [SerializeField] private float tryPeekBuffer;
    [SerializeField] private float tryPeekTimeMax;
    [SerializeField] private bool tryingToPeek;

    [Header("Fields - Peek")]
    [SerializeField] private float peekDistance;
    [SerializeField] private float startPeekTime;
    [SerializeField] private float endPeekTime;
    [SerializeField] private Vector2 peekDamp;
    [SerializeField] private bool peeking;
    [SerializeField] private bool changePeek;

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


        // Check if the camera is the default, follow camera
        if (!CameraManager.Instance.IsDefaultCam)
        {
            // Reset the peek variables
            return;
        }

        // Create rect if not created already
        if (noPeekZone == null)
            noPeekZone = new Rect();
        
        // Set bounds with added padding
        noPeekZone.xMin = currentBounds.Left + boundaryPadding;
        noPeekZone.xMax = currentBounds.Right - boundaryPadding;
        noPeekZone.yMax = currentBounds.Top - boundaryPadding;
        noPeekZone.yMin = currentBounds.Bottom + boundaryPadding;

        // Get cursor position
        Vector2 rawCursorPos = Camera.main.ScreenToWorldPoint(
            new Vector2(
                Mouse.current.position.ReadValue().x,
                Mouse.current.position.ReadValue().y
        )
        );

        cursorPos = Vector2Extensions.Round(rawCursorPos, 1);

        // Check if the cursor is within the bounds of the noPeekZone
        if (cursorPos.x < noPeekZone.xMin || cursorPos.x > noPeekZone.xMax ||
            cursorPos.y < noPeekZone.yMin || cursorPos.y > noPeekZone.yMax)
        {
            // If so, try to peek
            tryingToPeek = true;
        }
        else
        {
            // If not, then stop trying to peek
            tryingToPeek = false;

            // Check if peekiing
            if (peeking)
            {
                // Stop peeking
                CameraManager.Instance.Unpeek(endPeekTime);
            }

            // Reset peeking variables
            tryPeekBuffer = 0f;
            peeking = false;
        }

        // Check if we are trying to peek, but have not yet peeked
        if(tryingToPeek && !peeking)
        {
            // Increment the peek buffer
            tryPeekBuffer += Time.deltaTime;

            // Check if the player has been trying to peek for long enough
            if(tryPeekBuffer >= tryPeekTimeMax)
            {
                // Learn peeking
                onLearnPeek.Raise(this, null);

                // Set peeking to true
                peeking = true;
            }
        }

        // Check if the camera is peeking
        if(peeking)
        {
            // Get the current direction to the cursor from the follow object
            currentDirection = (cursorPos - currentFollowPos).normalized;

            // Get the calculated destination position
            calculatedFollowPos = Vector2Extensions.Round(currentFollowPos + (currentDirection * peekDistance), 1);

            // Get the direction to the calculated follow position
            calculatedDirection = calculatedFollowPos - currentFollowPos;

            // Peek towards that direction
            CameraManager.Instance.Peek(calculatedDirection.normalized, peekDamp, peekDistance, startPeekTime);
        }
    }

    /// <summary>
    /// Update the current camera bounds
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="data"></param>
    public void UpdateCurrentBounds(Component sender, object data)
    {
        // Make sure the correct data was sent
        if (data is not CameraData.ScreenBounds) return;

        // Cast and update data
        currentBounds = (CameraData.ScreenBounds)data;
    }

    public void UpdateFollowPosition(Component sender, object data)
    {
        // Make sure the correct data was sent
        if (data is not Vector2) return;

        // Cast and update data
        currentFollowPos = Vector2Extensions.Round((Vector2)data, 1);
    }

    private void OnDrawGizmos()
    {
        if(noPeekZone != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(noPeekZone.center, noPeekZone.size);
        }

        if(calculatedFollowPos != null && currentFollowPos != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(currentFollowPos, calculatedFollowPos);
        }
    }
}

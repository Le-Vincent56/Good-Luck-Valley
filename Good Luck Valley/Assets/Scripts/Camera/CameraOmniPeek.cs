using GoodLuckValley.Cameras;
using GoodLuckValley.Events;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

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
    [SerializeField] private float peekTime;
    [SerializeField] private Vector2 peekDamp;
    [SerializeField] private bool peeking;

    private Vector2 currentFollowPos;
    private Vector2 previousCursorPos;
    private Vector2 currentCursorPos;
    private Vector2 eventCursorPos;
    private Vector2 previousDirection;
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

        currentCursorPos = eventCursorPos;

        // Get direction to cursor
        Vector2 direction = (currentCursorPos - currentFollowPos).normalized;

        // Check if trying to peek
        if (currentCursorPos.x < noPeekZone.xMin || currentCursorPos.x > noPeekZone.xMax ||
            currentCursorPos.y < noPeekZone.yMin || currentCursorPos.y > noPeekZone.yMax)
        {
            tryingToPeek = true;
        } else
        {
            tryingToPeek = false;

            if(peeking)
            {
                // Pan the camera back
                CameraManager.Instance.Peek(peekDistance, peekTime, direction, peekDamp, true);
            }

            // TODO: Reset peeking
            tryPeekBuffer = 0f;
            peeking = false;
        }

        if (peeking && previousCursorPos == currentCursorPos) return;

        if (tryingToPeek)
        {
            tryPeekBuffer += Time.deltaTime;

            if(tryPeekBuffer >= tryPeekTimeMax)
            {
                // Learn peeking
                onLearnPeek.Raise(this, null);

                // Pan the camera
                CameraManager.Instance.Peek(peekDistance, peekTime, direction, peekDamp, false);
                peeking = true;
            }
        }

        previousCursorPos = currentCursorPos;
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

    /// <summary>
    /// Update the cursor position
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="data"></param>
    public void UpdateCursorPosition(Component sender, object data)
    {
        // Make sure the correct data was sent
        if (data is not Vector2) return;

        // Cast and update data
        eventCursorPos = (Vector2)data;
    }

    public void UpdateFollowPosition(Component sender, object data)
    {
        // Make sure the correct data was sent
        if (data is not Vector2) return;

        // Cast and update data
        currentFollowPos = (Vector2)data;
    }

    private void OnDrawGizmos()
    {
        if(noPeekZone != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(noPeekZone.center, noPeekZone.size);
        }
    }
}

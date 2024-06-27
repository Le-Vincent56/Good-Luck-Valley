using GoodLuckValley.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOmniPeek : MonoBehaviour
{
    [Header("Fields")]
    [SerializeField] private bool debug;
    [SerializeField] private Vector2 boundaryPadding;
    [SerializeField] private CameraData.ScreenBounds currentBounds;
    [SerializeField] private Rect noPeekZone;

    private void Update()
    {
        

        float width = Mathf.Abs(currentBounds.Right - currentBounds.Left);
        float height = Mathf.Abs(currentBounds.Top - currentBounds.Bottom);

        Debug.Log($"Left: {currentBounds.Left}, Right: {currentBounds.Right}");
        Debug.Log($"Top: {currentBounds.Top}, Bottom: {currentBounds.Bottom}");
        Debug.Log($"Width: {width}, Height: {height}");

        //noPeekZone = new Rect(
        //    currentBounds.Left + boundaryPadding.x,
        //    currentBounds.Top - boundaryPadding.y,

        //);
    }

    /// <summary>
    /// Update the bounds that the cursor is restricted to
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
}

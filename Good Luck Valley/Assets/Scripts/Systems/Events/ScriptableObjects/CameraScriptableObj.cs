using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "CameraScriptableObject", menuName = "ScriptableObjects/Camera Event")]
public class CameraScriptableObj : ScriptableObject
{
    #region REFERENCES
    [SerializeField] private Camera mainCamera;
    #endregion

    #region FIELDS
    [SerializeField] private Rect mainViewport;
    [SerializeField] private Rect cinemachineViewport;
    [SerializeField] private Rect currentCMViewport;
    [SerializeField] private Rect previousCMViewport;
    [SerializeField] private float camWidth;
    [SerializeField] private float camHeight;
    [SerializeField] private float vpWidthOffset;
    [SerializeField] private float vpHeightOffset;
    [SerializeField] private float leftBound;
    [SerializeField] private float rightBound;
    [SerializeField] private float topBound;
    [SerializeField] private float bottomBound;
    [SerializeField] private bool isCameraMoving;

    #region EVENTS
    public UnityEvent moveEvent;
    #endregion
    #endregion

    private void OnEnable()
    {
        #region CREATE EVENTS
        if (moveEvent == null)
        {
            moveEvent = new UnityEvent();
        }
        #endregion
    }

    /// <summary>
    /// Set the Main Camera
    /// </summary>
    /// <param name="mainCamera">The Main Camera</param>
    public void SetMainCamera(Camera mainCamera)
    {
        this.mainCamera = mainCamera;
    }

    /// <summary>
    /// Set the viewport of the Main Camera
    /// </summary>
    /// <param name="mainViewport">The viewport of the main camera</param>
    public void SetMainViewport(Rect mainViewport)
    {
        this.mainViewport = mainViewport;
    }

    /// <summary>
    /// Set the viewport of the Cinemachine camera
    /// </summary>
    /// <param name="cinemachineViewport">The viewport of the Cinemachine camera</param>
    public void SetCMViewport(Rect cinemachineViewport)
    {
        this.cinemachineViewport = cinemachineViewport;
    }

    /// <summary>
    /// Set the current Cinemachine viewport
    /// </summary>
    /// <param name="currentCMViewport">The current Cinemachine viewport</param>
    public void SetCurrentCMViewport(Rect currentCMViewport)
    {
        this.currentCMViewport = currentCMViewport;
    }

    /// <summary>
    /// Set the previous Cinemachine viewport
    /// </summary>
    /// <param name="previousCMViewport">The previous Cinemachine viewport</param>
    public void SetPreviousCMViewport(Rect previousCMViewport)
    {
        this.previousCMViewport = previousCMViewport;
    }

    /// <summary>
    /// Set the width of the Main Camera
    /// </summary>
    /// <param name="camWidth">The width of the Main Camera</param>
    public void SetCamWidth(float camWidth)
    {
        this.camWidth = camWidth;
    }

    /// <summary>
    /// Set the height of the Main Camera
    /// </summary>
    /// <param name="camHeight">The height of the Main Camera</param>
    public void SetCamHeight(float camHeight)
    {
        this.camHeight = camHeight;
    }

    /// <summary>
    /// Set the width offset between the Cinemachine and the Main Camera's viewports
    /// </summary>
    /// <param name="vpWidthOffset">The width offset between the Cinemachine and the Main Camera's viewports</param>
    public void SetViewportWidthOffset(float vpWidthOffset)
    {
        this.vpWidthOffset = vpWidthOffset;
    }

    /// <summary>
    /// Set the height offset between the Cinemachine and the Main Camera's viewports
    /// </summary>
    /// <param name="vpWidthOffset">The height offset between the Cinemachine and the Main Camera's viewports</param>
    public void SetViewportHeightOffset(float vpHeightOffset)
    {
        this.vpHeightOffset = vpHeightOffset;
    }

    /// <summary>
    /// Set the left bound of the Main Camera
    /// </summary>
    /// <param name="leftBound">The left bound of the Main Camera</param>
    public void SetLeftBound(float leftBound)
    {
        this.leftBound = leftBound;
    }

    /// <summary>
    /// Set the right bound of the Main Camera
    /// </summary>
    /// <param name="rightBound">The right bound of the Main Camera</param>
    public void SetRightBound(float rightBound)
    {
        this.rightBound = rightBound;
    }

    /// <summary>
    /// Set the top bound of the Main Camera
    /// </summary>
    /// <param name="topBound">The top bound of the Main Camera</param>
    public void SetTopBound(float topBound)
    {
        this.topBound = topBound;
    }

    /// <summary>
    /// Set the bottom bound of the Main Camera
    /// </summary>
    /// <param name="bottomBound">The bottom bound of the Main Camera</param>
    public void SetBottomBound(float bottomBound)
    {
        this.bottomBound = bottomBound;
    }

    /// <summary>
    /// Set whether the Main Camera is moving or not
    /// </summary>
    /// <param name="isCameraMoving">Whether the Main Camera is moving or not</param>
    public void SetCameraMoving(bool isCameraMoving)
    {
        this.isCameraMoving = isCameraMoving;
    }

    /// <summary>
    /// Get the Main Camera
    /// </summary>
    /// <returns>The Main Camera</returns>
    public Camera GetMainCamera()
    {
        return mainCamera;
    }

    /// <summary>
    /// Get the viewport of the Main Camera
    /// </summary>
    /// <returns>The viewport of the Main Camera</returns>
    public Rect GetMainViewport()
    {
        return mainViewport;
    }

    /// <summary>
    /// Get the viewport of the Cinemachine camera
    /// </summary>
    /// <returns>The viewport of the Cinemachine camera</returns>
    public Rect GetCMViewport()
    {
        return cinemachineViewport;
    }

    /// <summary>
    /// Get the current Cinemachine viewport
    /// </summary>
    /// <returns>The current Cinemachine viewport</returns>
    public Rect GetCurrentCMViewport()
    {
        return currentCMViewport;
    }

    /// <summary>
    /// Get the previous Cinemachine viewport
    /// </summary>
    /// <returns>The previous Cinemachine viewport</returns>
    public Rect GetPreviousCMViewport()
    {
        return previousCMViewport;
    }

    /// <summary>
    /// Get the width of the Main Camera
    /// </summary>
    /// <returns>The width of the Main Camera</returns>
    public float GetCamWidth()
    {
        return camWidth;
    }

    /// <summary>
    /// Get the height of the Main Camera
    /// </summary>
    /// <returns>The height of the Main Camera</returns>
    public float GetCamHeight()
    {
        return camHeight;
    }

    /// <summary>
    /// Get the width offset between the Cinemachine and the Main Camera's viewports
    /// </summary>
    /// <returns>The width offset between the Cinemachine and the Main Camera's viewports</returns>
    public float GetViewportWidthOffset()
    {
        return vpWidthOffset;
    }

    /// <summary>
    /// Get the height offset between the Cinemachine and the Main Camera's viewports
    /// </summary>
    /// <returns>The height offset between the Cinemachine and the Main Camera's viewports</returns>
    public float GetViewportHeightOffset()
    {
        return vpHeightOffset;
    }

    /// <summary>
    /// Get the left bound of the Main Camera
    /// </summary>
    /// <returns>The left bound of the Main Camera</returns>
    public float GetLeftBound()
    {
        return leftBound;
    }

    /// <summary>
    /// Get the right bound of the Main Camera
    /// </summary>
    /// <returns>The right bound of the Main Camera</returns>
    public float GetRightBound()
    {
        return rightBound;
    }

    /// <summary>
    /// Get the top bound of the Main Camera
    /// </summary>
    /// <returns>The top bound of the Main Camera</returns>
    public float GetTopBound()
    {
        return topBound;
    }

    /// <summary>
    /// Get the bottom bound of the Main Camera
    /// </summary>
    /// <returns>The bottom bound of the Main Camera</returns>
    public float GetBottomBound()
    {
        return bottomBound;
    }

    /// <summary>
    /// Get whether the Main Camera is moving or not
    /// </summary>
    /// <returns>Whether the Main Camera is moving or not</returns>
    public bool GetCameraMoving()
    {
        return isCameraMoving;
    }

    /// <summary>
    /// Trigger any events relating to moving the camera
    /// </summary>
    public void Move()
    {
        moveEvent.Invoke();
    }

    public void ResetObj()
    {
        mainViewport = Rect.zero;
        cinemachineViewport = Rect.zero;
        currentCMViewport = Rect.zero;
        previousCMViewport = Rect.zero;
        camWidth = 0f;
        camHeight = 0f;
        vpWidthOffset = 0f;
        vpHeightOffset = 0f;
        leftBound = 0f;
        rightBound = 0f;
        topBound = 0f;
        bottomBound = 0f;
        isCameraMoving = false;
    }
}

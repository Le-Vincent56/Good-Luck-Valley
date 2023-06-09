using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class GameCursor : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] Camera cam;
    private GameObject cmCam;
    private CinemachineVirtualCamera cineVirt;
    private GameObject player;
    private PauseMenu pauseMenu;
    #endregion

    #region FIELDS
    [SerializeField] private bool basedOnPlayer;
    private Rect cmViewport;
    private Rect mainViewport;
    private float camHeight;
    private float camWidth;
    private float widthOffset;
    private float heightOffset;
    private Vector2 cursorPosition;
    private Vector2 cursorVelocity = Vector3.zero;
    private Vector2 cursorDirection = Vector3.zero;
    private float cursorSpeed = 30f;
    private bool usingMouse = false;
    [SerializeField] private bool showDebugLines = false;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;

        // Get components
        cam = Camera.main;
        cmCam = GameObject.Find("CM vcam1");
        cineVirt = cmCam.GetComponent<CinemachineVirtualCamera>();

        if(basedOnPlayer)
        {
            player = GameObject.Find("Player");
            pauseMenu = GameObject.Find("PauseUI").GetComponent<PauseMenu>();
        }

        // Create bounds
        camHeight = cam.orthographicSize;
        camWidth = camHeight * cam.aspect;

        // Cursor fields
        cursorPosition = transform.position;

        // Confine the cursor to the window
        Cursor.lockState = CursorLockMode.Confined;
    }

    // Update is called once per frame
    void Update()
    {
        Cursor.visible = false;

        // Check if using mouse or gamepad input
        if (usingMouse)
        {
            // If using mouse, track mouse
            cursorDirection = Vector3.zero;
            cursorVelocity = Vector3.zero;
            cursorPosition = cam.ScreenToWorldPoint(new Vector2(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y));
        }
        else
        {
            // Update cursor positiond with player
            if (basedOnPlayer)
            {
                if (player.GetComponent<PlayerMovement>().IsMoving)
                {
                    cursorPosition += player.GetComponent<PlayerMovement>().DistanceFromLastPosition;
                }
            }

            // Move cursor with gamepad
            cursorVelocity = cursorDirection.normalized * cursorSpeed * Time.deltaTime;
            cursorPosition += cursorVelocity;
        }

        // Calculate bounds of both the Cinemachine Camera and the Main Camera
        // Get the rectangles of both cameras - encompassing their current bounds
        cmViewport = CalculateViewportRectangleCM();
        mainViewport = CalculateViewportRectangleMain();

        // Check bounds
        if (basedOnPlayer)
        {
            CheckCursorBoundsPlayer();
        }
        else
        {
            CheckCursorBoundsStatic();
        }


        // Draw cursor
        transform.position = cursorPosition;

        // For debugging
        if (showDebugLines)
        {
            Debug.DrawLine(new Vector2(cmViewport.xMin, cmViewport.yMin), new Vector2(cmViewport.xMax, cmViewport.yMax), Color.red);
            Debug.DrawLine(new Vector2(mainViewport.xMin, mainViewport.yMin), new Vector2(mainViewport.xMax, mainViewport.yMax), Color.blue);
        }
    }

    /// <summary>
    /// Check cursor bounds when there is a dynamic camera
    /// </summary>
    public void CheckCursorBoundsPlayer()
    {
        // Set default Main Camera bounds
        float leftBound = player.transform.position.x - camWidth;
        float rightBound = player.transform.position.x + camWidth;
        float lowerBound = player.transform.position.y - camHeight;
        float upperBound = player.transform.position.y + camHeight;

        // Calculate width offset
        // Negative values - Cinemachine is to the left of the main Camera
        // Positive values - Cinemachine is to the right of the main Camera
        widthOffset = cmViewport.x - mainViewport.x;
        leftBound = cmViewport.xMin - widthOffset;
        rightBound = cmViewport.xMax - widthOffset;


        // Calculate height offset
        // Negative values - Cinemachine is below the main Camera
        // Positive values - Cinemachine is above the main Camera
        heightOffset = cmViewport.y - mainViewport.y;
        lowerBound = cmViewport.yMin - heightOffset;
        upperBound = cmViewport.yMax - heightOffset; 

        // Clamp the cursor position into the bounds of the screen
        cursorPosition.x = Mathf.Clamp(cursorPosition.x, leftBound, rightBound);
        cursorPosition.y = Mathf.Clamp(cursorPosition.y, lowerBound, upperBound);
    }

    /// <summary>
    /// Check cursor bounds when there is a static camera
    /// </summary>
    public void CheckCursorBoundsStatic()
    {
        cursorPosition.x = Mathf.Clamp(cursorPosition.x, -camWidth, camWidth);
        cursorPosition.y = Mathf.Clamp(cursorPosition.y, -camHeight, camHeight);
    }

    private Rect CalculateViewportRectangleCM()
    {
        float orthographicSize = cineVirt.m_Lens.OrthographicSize;
        float aspectRatio = cineVirt.m_Lens.Aspect;

        float viewportHeight = orthographicSize * 2;
        float viewportWidth = viewportHeight * aspectRatio;

        Vector2 cameraPosition = cineVirt.transform.position;

        Rect viewportRect = new Rect(
            cameraPosition.x - viewportWidth / 2,
            cameraPosition.y - viewportHeight / 2,
            viewportWidth,
            viewportHeight);

        return viewportRect;
    }

    private Rect CalculateViewportRectangleMain()
    {
        Vector2 cameraPosition = cam.transform.position;

        float orthographicSize = cam.orthographicSize;
        float aspectRatio = cam.aspect;

        float camHeight = orthographicSize * 2;
        float camWidth = camHeight * aspectRatio;

        Rect viewportRect = new Rect(
            cam.transform.position.x - camWidth / 2,
            cam.transform.position.y - camHeight / 2,
            camWidth,
            camHeight);

        return viewportRect;
    }

    #region INPUT HANDLER
    /// <summary>
    /// OnAim event to trigger cursor movement
    /// </summary>
    /// <param name="context">The current controller context being used</param>
    public void OnAim(InputAction.CallbackContext context)
    {
        // Check if context is mouse or controller
        if (context.control.name == "position")
        {
            usingMouse = true;
        }
        else
        {
            // Controller read values
            usingMouse = false;
            cursorDirection = context.ReadValue<Vector2>();
        }
    }
    #endregion
}

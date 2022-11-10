using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class GameCursor : MonoBehaviour
{
    #region FIELDS
    [Header("Camera")]
    private Camera cam;
    private GameObject cmCam;

    private float cmWidth;
    private float cmHeight;

    private float camHeight;
    private float camWidth;

    [SerializeField] float widthOffset;
    private float heightOffset;

    [SerializeField] float widthDampen;
    private float heightDampen;

    [Header("Player")]
    GameObject player;

    [Header("Cursor")]
    public bool activated = false;
    public Vector2 cursorPosition;
    public Vector2 cursorVelocity = Vector3.zero;
    public Vector2 cursorDirection = Vector3.zero;
    public float cursorSpeed = 30f;
    public bool usingMouse = false;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;

        // Get components
        cam = Camera.main;
        cmCam = GameObject.Find("CM vcam1");
        player = GameObject.Find("Player");

        // Create bounds
        camHeight = cam.orthographicSize;
        camWidth = camHeight * cam.aspect;

        // Cursor fields
        cursorPosition = transform.position;
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
            if (player.GetComponent<PlayerMovement>()._isMoving)
            {
                cursorPosition += player.GetComponent<PlayerMovement>().distanceFromLastPosition;
            }

            // Move cursor with gamepad
            cursorVelocity = cursorDirection.normalized * cursorSpeed * Time.deltaTime;
            cursorPosition += cursorVelocity;
        }

        // Check bounds
        CheckCursorBounds();

        // Draw cursor
        transform.position = cursorPosition;
    }

    /// <summary>
    /// Check cursor bounds
    /// </summary>
    public void CheckCursorBounds()
    {
        // Set default Main Camera bounds
        float leftBound = player.transform.position.x - camWidth;
        float rightBound = player.transform.position.x + camWidth;
        float lowerBound = player.transform.position.y - camHeight;
        float upperBound = player.transform.position.y + camHeight;

        #region WIDTH BOUNDS
        // Calculate the orthographic width of the Cinemachine Virtual Camera
        cmWidth = (player.transform.position.x
            - (cmCam.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize * cmCam.GetComponent<CinemachineVirtualCamera>().m_Lens.Aspect))
            - cmCam.transform.position.x;


        // Calculate Cinemachine's Left Bound
        float cmLeftBound = cmCam.transform.position.x + Mathf.Abs(cmWidth);

        // Calculate the Main Camera's Left Bound
        float mcLeftBound = cam.transform.position.x + Mathf.Abs(camWidth);

        // Compare the width offset between the two cameras
        widthOffset = cmLeftBound - mcLeftBound;

        // Get the current width of the Main Camera
        float leftWidth = leftBound - player.transform.position.x;

        // Subtract the absolute value of the dampening height
        // from the absolute value of the main camera height
        widthDampen = Mathf.Abs(cmWidth) - Mathf.Abs(leftWidth);

        if(Mathf.Abs(widthOffset) - Mathf.Abs(widthDampen) < 0.01)
        {
            // If there is a minimal difference in offset and dampen, just add dampen
            leftBound = player.transform.position.x - camWidth + widthDampen;
            rightBound = player.transform.position.x + camWidth + widthDampen;
        } else
        {
            // Otherwise, add offset and dampen
            leftBound = player.transform.position.x - camWidth - widthOffset + (2 * widthDampen);
            rightBound = player.transform.position.x + camWidth - widthOffset + (2 * widthDampen);
        }
        #endregion

        #region HEIGHT BOUNDS
        // Calculate the orthographic height of the Cinemachine Virtual Camera
        cmHeight = (player.transform.position.y 
            - (cmCam.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize)) 
            - cmCam.transform.position.y;

        // Calculate Cinemachine's Bottom Bound
        float cmBottomBound = cmCam.transform.position.y + Mathf.Abs(cmHeight);

        // Calculate the Main Camera's Bottom Bound
        float mcBottomBound = cam.transform.position.y + Mathf.Abs(camHeight);

        // Compare the height offset between the two cameras
        heightOffset = Mathf.Abs(cmBottomBound - mcBottomBound);

        // Get the current height of the Main Camera
        float bottomHeight = lowerBound - player.transform.position.y;

        // Subtract the absolute value of the dampening height
        // from the absolute value of the main camera height
        heightDampen = Mathf.Abs(cmHeight) - Mathf.Abs(bottomHeight);

        if (Mathf.Abs(heightOffset) - Mathf.Abs(heightDampen) < 0.01)
        {
            // If there is a minimal difference in offset and dampen, just add dampen
            lowerBound = player.transform.position.y - camHeight + heightDampen;
            upperBound = player.transform.position.y + camHeight + heightDampen;
        }
        else
        {
            // Otherwise, add offset and dampen
            lowerBound = player.transform.position.y - camHeight - heightOffset + (2 * widthDampen);
            upperBound = player.transform.position.y + camHeight - heightOffset + (2 * widthDampen);
        }
        #endregion

        // Check x bounds against camera
        if (cursorPosition.x < leftBound)
        {
            cursorPosition.x = leftBound;
        }
        else if (cursorPosition.x > rightBound)
        {
            cursorPosition.x = rightBound;
        }

        // Check y bounds against camera
        if (cursorPosition.y < lowerBound)
        {
            cursorPosition.y = lowerBound;
        }
        else if (cursorPosition.y > upperBound)
        {
            cursorPosition.y = upperBound;
        }
    }

    #region INPUT HANDLER
    public void OnAim(InputAction.CallbackContext context)
    {
        // Check if context is mouse or controller
        if(context.control.name == "position")
        {
            usingMouse = true;
        } else
        {
            // Controller read values
            usingMouse = false;
            cursorDirection = context.ReadValue<Vector2>();
        }
    }
    #endregion
}

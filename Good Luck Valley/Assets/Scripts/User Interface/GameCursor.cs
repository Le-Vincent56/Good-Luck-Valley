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
    private Transform vcamTrans;
    private float camHeight;
    private float camWidth;

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
        vcamTrans = cam.GetComponentInChildren<Transform>();
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
        // Check if using mouse or gamepad input
        if (usingMouse)
        {
            // If using mouse, track mouse
            cursorDirection = Vector3.zero;
            cursorVelocity = Vector3.zero;
            cursorPosition = cam.ScreenToWorldPoint(new Vector2(Mouse.current.position.ReadValue().x, Mouse.current.position.ReadValue().y));
        } else
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
        float leftBound = player.transform.position.x - camWidth;
        float rightBound = player.transform.position.x + camWidth;
        float lowerBound = player.transform.position.y - camHeight;
        float upperBound = player.transform.position.y + camHeight;


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

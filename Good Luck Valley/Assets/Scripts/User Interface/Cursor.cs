using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Cursor : MonoBehaviour
{
    #region FIELDS
    [Header("Camera")]
    Camera cam;
    float camHeight;
    float camWidth;

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
        // Get components
        cam = Camera.main;
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
        
        // Draw cursor
        transform.position = cursorPosition;

        // Check bounds
        CheckCursorBounds();
    }

    /// <summary>
    /// Check cursor bounds
    /// </summary>
    public void CheckCursorBounds()
    {
        // Check x bounds against camera
        if (cursorPosition.x < -camWidth)
        {
            cursorPosition.x = -camWidth;
        }
        else if (cursorPosition.x > camWidth)
        {
            cursorPosition.x = camWidth;
        }

        // Check y bounds against camera
        if (cursorPosition.y < -camHeight)
        {
            cursorPosition.y = -camHeight;
        }
        else if (cursorPosition.y > camHeight)
        {
            cursorPosition.y = camHeight;
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

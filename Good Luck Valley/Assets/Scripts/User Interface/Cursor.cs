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
        // Update cursor positiond
        if (player.GetComponent<PlayerMovement>()._isMoving)
        {
            cursorPosition += player.GetComponent<PlayerMovement>().distanceFromLastPosition;
        }
        cursorVelocity = cursorDirection.normalized * cursorSpeed * Time.deltaTime;
        cursorPosition += cursorVelocity;
        transform.position = cursorPosition;

        CheckCursorBounds();
    }

    public void CheckCursorBounds()
    {
        if (cursorPosition.x < -camWidth)
        {
            cursorPosition.x = -camWidth;
        }
        else if (cursorPosition.x > camWidth)
        {
            cursorPosition.x = camWidth;
        }

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
        // Implement looking

        // Check if context is mouse or controller

        // Mouse -> ScreenSpace to WorldSpace to LocalSpace

        // Controller read values
        cursorDirection = context.ReadValue<Vector2>();
        Debug.Log(cursorDirection);
    }
    #endregion
}

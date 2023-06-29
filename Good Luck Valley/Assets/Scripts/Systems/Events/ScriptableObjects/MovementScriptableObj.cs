using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "MovementScriptableObject", menuName = "ScriptableObjects/Movement Event")]
public class MovementScriptableObj : ScriptableObject
{
    #region FIELDS
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isFalling;
    [SerializeField] private bool isLanding;
    [SerializeField] private Vector2 movementDirection;
    [SerializeField] private Vector2 inputDirection;

    #region EVENTS
    [System.NonSerialized]
    public UnityEvent<float> moveEvent;
    public UnityEvent<bool> jumpEvent;
    public UnityEvent<bool> fallEvent;
    public UnityEvent<bool> landEvent;
    public UnityEvent<float, float, bool> footstepEvent;
    #endregion
    #endregion

    private void OnEnable()
    {
        // Create all events
        #region CREATE EVENTS
        if (moveEvent == null)
        {
            moveEvent = new UnityEvent<float>();
        }

        if(jumpEvent == null)
        {
            jumpEvent = new UnityEvent<bool>();
        }

        if (fallEvent == null)
        {
            fallEvent = new UnityEvent<bool>();
        }

        if (landEvent == null)
        {
            landEvent = new UnityEvent<bool>();
        }

        if(footstepEvent == null)
        {
            footstepEvent = new UnityEvent<float, float, bool>();
        }
        #endregion
    }

    /// <summary>
    /// Set movement bools
    /// </summary>
    /// <param name="isGrounded">Whether the player is grounded</param>
    /// <param name="isJumping">Whether the player is jumping</param>
    /// <param name="isFalling">Whether the player is falling</param>
    /// <param name="isLanding">Whether the player is landing</param>
    public void SetBools(bool isGrounded, bool isJumping, bool isFalling, bool isLanding)
    {
        this.isGrounded = isGrounded;
        this.isJumping = isJumping;
        this.isFalling = isFalling;
        this.isLanding = isLanding;
    }

    /// <summary>
    /// Set movement vectors
    /// </summary>
    /// <param name="movementDirection">The rigidbody movement direction</param>
    /// <param name="inputDirection">The player input direction</param>
    public void SetVectors(Vector2 movementDirection, Vector2 inputDirection)
    {
        this.movementDirection = movementDirection;
        this.inputDirection = inputDirection;
    }

    /// <summary>
    /// Trigger movement-related events
    /// </summary>
    public void Move()
    {
        moveEvent.Invoke(movementDirection.x);
        footstepEvent.Invoke(inputDirection.x, movementDirection.x, isGrounded);
    }

    /// <summary>
    /// Trigger jump-related events
    /// </summary>
    public void Jump()
    {
        jumpEvent.Invoke(isJumping);
    }

    /// <summary>
    /// Trigger fall-related events
    /// </summary>
    public void Fall()
    {
        fallEvent.Invoke(isFalling);
    }

    /// <summary>
    /// Trigger landing-related events
    /// </summary>
    public void Land()
    {
        landEvent.Invoke(isLanding);
    }
}

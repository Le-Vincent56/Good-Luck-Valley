using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "MovementScriptableObject", menuName = "ScriptableObjects/Movement Event")]
public class MovementScriptableObj : ScriptableObject, IData
{
    #region FIELDS
    [SerializeField] private PlayerState currentState;
    [SerializeField] private PlayerState prevState;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isFalling;
    [SerializeField] private bool isLanding;
    [SerializeField] private bool isBouncing;
    [SerializeField] private bool isBounceAnimating;
    [SerializeField] private bool isTouchingWall;
    [SerializeField] private Vector2 movementDirection;
    [SerializeField] private Vector2 inputDirection;
    [SerializeField] private Vector3 mushroomPosition;
    [SerializeField] private TileType movementTileType;

    #region EVENTS
    [System.NonSerialized]
    public UnityEvent moveEvent;
    public UnityEvent jumpEvent;
    public UnityEvent fallEvent;
    public UnityEvent landEvent;
    public UnityEvent<Vector3, ForceMode2D> bounceEvent;
    public UnityEvent bounceAnimationEvent;
    public UnityEvent<float, float, bool, TileType> footstepEvent;
    public UnityEvent<float, bool, TileType> startFootstepEventCutscene;
    public UnityEvent<float, bool, TileType> stopFootstepEventCutscene;
    public UnityEvent<int> setTurnDirection;
    public UnityEvent resetTurn;
    public UnityEvent<Vector2> applyMovementDirection;
    #endregion
    #endregion

    private void OnEnable()
    {
        // Create all events
        #region CREATE EVENTS
        if (moveEvent == null)
        {
            moveEvent = new UnityEvent();
        }

        if(jumpEvent == null)
        {
            jumpEvent = new UnityEvent();
        }

        if (fallEvent == null)
        {
            fallEvent = new UnityEvent();
        }

        if (landEvent == null)
        {
            landEvent = new UnityEvent();
        }

        if (bounceEvent == null)
        {
            bounceEvent = new UnityEvent<Vector3, ForceMode2D>();
        }

        if (bounceAnimationEvent == null)
        {
            bounceAnimationEvent = new UnityEvent();
        }

        if (footstepEvent == null)
        {
            footstepEvent = new UnityEvent<float, float, bool, TileType>();
        }

        if(startFootstepEventCutscene == null)
        {
            startFootstepEventCutscene = new UnityEvent<float, bool, TileType>();
        }

        if (stopFootstepEventCutscene == null)
        {
            stopFootstepEventCutscene = new UnityEvent<float, bool, TileType>();
        }

        if (setTurnDirection == null)
        {
            setTurnDirection = new UnityEvent<int>();
        }

        if(resetTurn == null)
        {
            resetTurn = new UnityEvent();
        }

        if(applyMovementDirection == null)
        {
            applyMovementDirection = new UnityEvent<Vector2>();
        }
        #endregion
    }

    private void Awake()
    {
        isGrounded = false;
        isJumping = false;
        isFalling = false;
        isLanding = false;
        isBouncing = false;
        isTouchingWall = false;
    }

    /// <summary>
    /// Set the current movement state of the player
    /// </summary>
    /// <param name="currentState">The current movement state of the player</param>
    public void SetCurrentState(PlayerState currentState)
    {
        this.currentState = currentState;
    }

    /// <summary>
    /// Set the previous movement state of the player
    /// </summary>
    /// <param name="prevState">The previous movement state of the player</param>
    public void SetPreviousState(PlayerState prevState)
    {
        this.prevState = prevState;
    }

    /// <summary>
    /// Set whether the player is grounded or not
    /// </summary>
    /// <param name="isGrounded">Whether the player is grounded or not</param>
    public void SetIsGrounded(bool isGrounded)
    {
        this.isGrounded = isGrounded;
    }

    /// <summary>
    /// Set whether the player is jumping or not
    /// </summary>
    /// <param name="isJumping">Whether the player is jumping or not</param>
    public void SetIsJumping(bool isJumping)
    {
        this.isJumping = isJumping;
    }

    /// <summary>
    /// Set whether the player is falling or not
    /// </summary>
    /// <param name="isFalling">Whether the player is falling or not</param>
    public void SetIsFalling(bool isFalling)
    {
        this.isFalling = isFalling;
    }

    /// <summary>
    /// Set whether the player is landing or not
    /// </summary>
    /// <param name="isLanding">Whether the player is landing or not</param>
    public void SetIsLanding(bool isLanding)
    {
        this.isLanding = isLanding;
    }

    /// <summary>
    /// Set whether the player is bouncing or not
    /// </summary>
    /// <param name="isBouncing">Whether the player is bouncing or not</param>
    public void SetIsBouncing(bool isBouncing)
    {
        this.isBouncing = isBouncing;
    }

    /// <summary>
    /// Set whether the player animation is set to bouncing or not
    /// </summary>
    /// <param name="isBounceAnimating">Whether the player animation is set to bouncing or not</param>
    public void SetIsBounceAnimating(bool isBounceAnimating)
    {
        this.isBounceAnimating = isBounceAnimating;
    }

    /// <summary>
    /// Set whether the player is touching wall jump wall
    /// </summary>
    /// <param name="isTouchingWall">Whether the player is touching wall</param>
    public void SetIsTouchingWall(bool isTouchingWall)
    {
        this.isTouchingWall = isTouchingWall;
    }

    public void SetMushroomPosition(Vector3 position)
    {
        this.mushroomPosition = position;
    }

    /// <summary>
    /// Set the movement direction of the player
    /// </summary>
    /// <param name="movementDirection">The movement direction of the player</param>
    public void SetMovementDirection(Vector2 movementDirection)
    {
        this.movementDirection = movementDirection;
    }

    /// <summary>
    /// Set the current tile type the player is moving on
    /// </summary>
    /// <param name="tileType"></param>
    public void SetTileType(TileType tileType)
    {
        movementTileType = tileType;
    }

    /// <summary>
    /// Get the current movement state of the player
    /// </summary>
    /// <returns>The current movement state of the player</returns>
    public PlayerState GetCurrentState()
    {
        return currentState;
    }

    /// <summary>
    /// Get the previous movement state of the player
    /// </summary>
    /// <returns>The previous movement state of the player</returns>
    public PlayerState GetPreviousState()
    {
        return prevState;
    }

    public Vector3 GetMushroomPosition()
    {
        return mushroomPosition;
    }

    /// <summary>
    /// Get whether the player is touching wall jump wall
    /// </summary>
    /// <returns> Whether the player is touching wall or not</returns>
    public bool GetIsTouchingWall()
    {
        return isTouchingWall;
    }

    /// <summary>
    /// Get whether the player is grounded or not
    /// </summary>
    /// <returns>Whether the player is grounded or not</returns>
    public bool GetIsGrounded()
    {
        return isGrounded;
    }

    /// <summary>
    /// Get whether the player is jumping or not
    /// </summary>
    /// <returns>Whether the player is jumping or not</returns>
    public bool GetIsJumping()
    {
        return isJumping;
    }

    /// <summary>
    /// Get whether the player is falling or not
    /// </summary>
    /// <returns>Whether the player is falling or not</returns>
    public bool GetIsFalling()
    {
        return isFalling;
    }

    /// <summary>
    /// Get whether the player is landing or not
    /// </summary>
    /// <returns>Whether the player is landing or not</returns>
    public bool GetIsLanding()
    {
        return isLanding;
    }

    /// <summary>
    /// Get whether the player is bouncing or not
    /// </summary>
    /// <returns>Whether the player is bouncing or not</returns>
    public bool GetBouncing()
    {
        return isBouncing;
    }

    /// <summary>
    /// Get whether the player is animating a bounce or not
    /// </summary>
    /// <returns>Whether the player is animating a bounce or not</returns>
    public bool GetIsBounceAnimating()
    {
        return isBounceAnimating;
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
    /// Get the player movement Vector2
    /// </summary>
    /// <returns>The player movement Vector2</returns>
    public Vector2 GetMovementVector()
    {
        return movementDirection;
    }

    /// <summary>
    /// Get the player input Vector2
    /// </summary>
    /// <returns>Player input Vector2</returns>
    public Vector2 GetInputVector()
    {
        return inputDirection;
    }

    /// <summary>
    /// Get the current tile type the player is moving on
    /// </summary>
    /// <returns>The current tile type the player is moving on</returns>
    public TileType GetTileType()
    {
        return movementTileType;
    }

    /// <summary>
    /// Trigger movement-related events
    /// </summary>
    public void Move()
    {
        moveEvent.Invoke();
        footstepEvent.Invoke(inputDirection.x, movementDirection.x, isGrounded, movementTileType);
    }

    /// <summary>
    /// Trigger any events related to the starting the footstep cutscene event
    /// </summary>
    public void StartCutsceneFootstepEvent()
    {
        startFootstepEventCutscene.Invoke(movementDirection.x, isGrounded, movementTileType);
    }

    public void StopCutsceneFootstepEvent()
    {
        stopFootstepEventCutscene.Invoke(movementDirection.x, isGrounded, movementTileType);
    }

    /// <summary>
    /// Trigger jump-related events
    /// </summary>
    public void Jump()
    {
        jumpEvent.Invoke();
    }

    /// <summary>
    /// Trigger fall-related events
    /// </summary>
    public void Fall()
    {
        fallEvent.Invoke();
    }

    /// <summary>
    /// Trigger landing-related events
    /// </summary>
    public void Land()
    {
        landEvent.Invoke();
    }

    /// <summary>
    /// Trigger bounce-related events
    /// </summary>
    public void Bounce(Vector3 forceToApply, ForceMode2D forceType)
    {
        bounceEvent.Invoke(forceToApply, forceType);
        bounceAnimationEvent.Invoke();
    }

    /// <summary>
    /// Set the player's turn direction
    /// </summary>
    /// <param name="direction">The turn direction, 1 for right, -1 for left</param>
    public void SetTurnDirection(int direction)
    {
        setTurnDirection.Invoke(direction);
    }

    /// <summary>
    /// Trigger events relating to resetting the player turn direction
    /// </summary>
    public void ResetTurn()
    {
        resetTurn.Invoke();
    }

    /// <summary>
    /// Apply movement direction to the player
    /// </summary>
    public void ApplyMovementDirection()
    {
        applyMovementDirection.Invoke(movementDirection);
    }

    /// <summary>
    /// Reset object variables
    /// </summary>
    public void ResetObj()
    {
        isGrounded = false;
        isJumping = false;
        isFalling = false;
        isLanding = false;
        isBouncing = false;
        isBounceAnimating = false;
        movementDirection = Vector3.zero;
        inputDirection = Vector3.zero;
    }

    #region DATA HANDLING
    public void LoadData(GameData data)
    {
        isGrounded = data.isGrounded;
        isJumping = data.isJumping;
        isFalling = data.isFalling;

        isBounceAnimating = false;
    }

    public void SaveData(GameData data)
    {
        data.isGrounded = isGrounded;
        data.isJumping = isJumping;
        data.isFalling = isFalling;
    }
    #endregion
}

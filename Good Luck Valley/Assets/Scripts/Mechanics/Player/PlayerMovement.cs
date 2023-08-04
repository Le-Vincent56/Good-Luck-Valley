using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using FMOD.Studio;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;
using UnityEditor.Rendering;

public enum PlayerState
{
    Idle,
    Running,
    Jumping,
    Falling,
    Bouncing
}

public class PlayerMovement : MonoBehaviour, IData
{
    #region REFERENCES
    [SerializeField] private PlayerData data;
    [SerializeField] private MovementScriptableObj movementEvent;
    [SerializeField] private MushroomScriptableObj mushroomEvent;
    [SerializeField] private DisableScriptableObj disableEvent;
    [SerializeField] private CutsceneScriptableObj cutsceneEvent;
    [SerializeField] private PauseScriptableObj pauseEvent;
    [SerializeField] private LoadLevelScriptableObj loadLevelEvent;
    [SerializeField] private LevelDataObj levelDataObj;
    [SerializeField] private PlayerInput playerInput;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject playerLight;
	[SerializeField] private Rigidbody2D rb;
	private BoxCollider2D playerCollider;
	private CapsuleCollider2D capsuleCollider;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
	[SerializeField] private PhysicsMaterial2D noFriction;
	[SerializeField] private PhysicsMaterial2D fullFriction;
	private DevTools devTools;
	private Settings settings;
    private VisualEffect dust;
    private VisualEffect grass;
    [SerializeField] private GameObject wallShroomPrefab;
    #endregion

    #region FIELDS
    [Header("General")]
    [SerializeField] private PlayerState currentState;
    [SerializeField] private PlayerState previousState;
    [SerializeField] bool debug = false;
    [SerializeField] bool isJumping;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isLocked = false;
    [SerializeField] private bool canInput = true;
    [SerializeField] private bool canInputHard = true;
    [SerializeField] float fallingBuffer = 0.25f;
    [SerializeField] float landedTimer = 0f;
    [SerializeField] private float inputCooldown = 0.05f;
    [SerializeField] private float jumpBuffer = 0f;
    private bool isMoving;
    private bool isJumpCut;
	private bool isJumpFalling;
    private bool isJumpAnimFalling;
	private bool isFacingRight;
    [SerializeField] private bool landed;
    private float lastOnGroundTime;
    private float lastPressedJumpTime;
    private Vector2 playerPosition;
    private Vector2 previousPlayerPosition;
    private Vector2 distanceFromLastPosition;

    // For creating dust particles when the player falls, if you find a better solution
    //  feel free to fuck with it this is just the only way I could figure it out
    private bool createDustOnFall;

    #region WALLS
    [Header("Walls")]
    [SerializeField] private bool debugWall;
    [SerializeField] private float wallStickForce;
    [SerializeField] private bool previousWallState;
    [SerializeField] private float wallStickTimerMax;
    [SerializeField] private float wallStickTimer = -1f;
    #endregion

    #region SLOPES
    [Header("Slopes")]
    [SerializeField] bool checkForSlopes = false;
    [SerializeField] bool isOnSlope;
    [SerializeField] bool canWalkOnSlope;
    [SerializeField] float slopeCheckDistance;
    [SerializeField] float slopeForceMagnitude = 5f;
    [SerializeField] float maxSlopeAngle;
    [SerializeField] private float currentSlopeDownAngle;
    [SerializeField] private float currentSlopeSideAngle;
    [SerializeField] private Vector2 moveInput;
    private Vector2 capsuleColliderSize;
	private Vector2 slopeNormal;
    private Vector3 slopeNormalPerp;
    private float slopeSideAngle;
	private float slopeDownAngle;
	private float slopeNormalPerpAngle;
    #endregion

    #region BOUNCING
    [Header("Bouncing")]
	[SerializeField] private bool bouncing = false;
    [SerializeField] private bool touchingShroom = false;
    [SerializeField] private float bounceBuffer = 0.1f;
	#endregion
	#endregion

	#region PROPERTIES
    public Rigidbody2D RB { get { return rb; } set { rb = value; } }
	public bool IsFacingRight { get { return isFacingRight; } set { isFacingRight = value; } }
    public bool IsMoving { get { return isMoving; } set { isMoving = value; } }
    public Vector2 DistanceFromLastPosition { get { return distanceFromLastPosition; } }

    public bool IsGrounded { get { return isGrounded; } }
    #endregion

    private void Awake()
	{
        playerInput = GetComponent<PlayerInput>();
		RB = GetComponent<Rigidbody2D>();
		spriteRenderer = GameObject.Find("PlayerSprite").GetComponent<SpriteRenderer>();
		playerCollider = GetComponent<BoxCollider2D>();
		capsuleCollider = GetComponent<CapsuleCollider2D>();
		devTools = GameObject.Find("Dev Tools").GetComponent<DevTools>();
		settings = GameObject.Find("MenusManager").GetComponent<Settings>();
    }

    private void OnEnable()
    {
        movementEvent.bounceEvent.AddListener(ApplyBounce);
        movementEvent.landEvent.AddListener(Land);
        movementEvent.resetTurn.AddListener(ResetTurn);
        movementEvent.applyMovementDirection.AddListener(SetMovementDirection);
        movementEvent.setTurnDirection.AddListener(SetTurnDirection);
        mushroomEvent.touchingShroomEvent.AddListener(TouchingShroom);
        pauseEvent.pauseEvent.AddListener(LockMovement);
        pauseEvent.unpauseEvent.AddListener(UnlockMovement);
        disableEvent.lockPlayerEvent.AddListener(LockMovement);
        disableEvent.unlockPlayerEvent.AddListener(UnlockMovement);
        disableEvent.stopInputEvent.AddListener(StopInput);
        disableEvent.disablePlayerInputEvent.AddListener(DisableInput);
        disableEvent.enablePlayerInputEvent.AddListener(EnableInput);
        loadLevelEvent.startLoad.AddListener(SetLoadPos);
        loadLevelEvent.startLoad.AddListener(LockMovement);
        loadLevelEvent.endLoad.AddListener(UnlockMovement);
        cutsceneEvent.startLotusCutscene.AddListener(LockMovement);
        cutsceneEvent.endLotusCutscene.AddListener(UnlockMovement);
    }

    private void OnDisable()
    {
        movementEvent.bounceEvent.RemoveListener(ApplyBounce);
        movementEvent.landEvent.RemoveListener(Land);
        movementEvent.resetTurn.RemoveListener(ResetTurn);
        movementEvent.applyMovementDirection.RemoveListener(SetMovementDirection);
        movementEvent.setTurnDirection.RemoveListener(SetTurnDirection);
        mushroomEvent.touchingShroomEvent.RemoveListener(TouchingShroom);
        pauseEvent.pauseEvent.RemoveListener(LockMovement);
        pauseEvent.unpauseEvent.RemoveListener(UnlockMovement);
        disableEvent.lockPlayerEvent.RemoveListener(LockMovement);
        disableEvent.unlockPlayerEvent.RemoveListener(UnlockMovement);
        disableEvent.stopInputEvent.RemoveListener(StopInput);
        disableEvent.disablePlayerInputEvent.RemoveListener(DisableInput);
        disableEvent.enablePlayerInputEvent.RemoveListener(EnableInput);
        loadLevelEvent.startLoad.RemoveListener(SetLoadPos);
        loadLevelEvent.startLoad.RemoveListener(LockMovement);
        loadLevelEvent.endLoad.RemoveListener(UnlockMovement);
        cutsceneEvent.startLotusCutscene.RemoveListener(LockMovement);
        cutsceneEvent.endLotusCutscene.RemoveListener(UnlockMovement);
    }

    private void Start()
	{
		SetGravityScale(data.gravityScale);
		isFacingRight = true;
		playerPosition = transform.position;
		playerLight = GameObject.Find("PlayerLight");
		capsuleColliderSize = capsuleCollider.size;
        wallStickTimer = -1f;
    }

    private void Update()
	{
        // Set playerPosition to the current position and calculate the distance from the previous position
        playerPosition = transform.position;
        distanceFromLastPosition = playerPosition - previousPlayerPosition;

		// Check if the player is moving using RB.velocity
        if (RB.velocity != Vector2.zero)
        {
            isMoving = true;
        } else
        {
            isMoving = false;
            currentState = PlayerState.Idle;
            movementEvent.SetCurrentState(currentState);
        }

		// Set the playerLight's position to the player's position
        playerLight.transform.position = transform.position;

		// Update timers
        #region TIMERS
        lastOnGroundTime -= Time.deltaTime;
        lastPressedJumpTime -= Time.deltaTime;

        // If the player is falling, update the fallingBuffer
        if (RB.velocity.y < 0 && !isGrounded)
        {
			fallingBuffer -= Time.deltaTime;
        }

        if (bounceBuffer > 0 && bouncing)
        {
            bounceBuffer -= Time.deltaTime;
        }

        if(wallStickTimer > 0)
        {
            wallStickTimer -= Time.deltaTime;
        }
        #endregion

        // Check for Collisions
        #region COLLISION CHECKS
        if (!isJumping)
        {
			RaycastHit2D boxCheckGround = Physics2D.BoxCast(GameObject.Find("PlayerSprite").GetComponent<BoxCollider2D>().bounds.center, new Vector3(playerCollider.bounds.size.x - 0.1f, playerCollider.bounds.size.y, playerCollider.bounds.size.z), 0f, Vector2.down, 0.1f, groundLayer);

            if (boxCheckGround && !touchingShroom && !isJumping) // Checks if set box overlaps with ground while not touching the shroom
            {
                // If bouncing before and the bounce buffer has ended, end bouncing
                if (bouncing && bounceBuffer <= 0)
                {
                    disableEvent.EnableInput();
                    bouncing = false;
                    movementEvent.SetIsBounceAnimating(false);
                }

                // Ground player
                isGrounded = true;

                // Set coyote time
                lastOnGroundTime = data.coyoteTime;
            }
        }

        // Check to see if a wall is being touched
        CheckForWall();

        // Check for is the player is on a wall
        if (movementEvent.GetIsTouchingWall())
        {
            // If bouncing before and the bounce buffer has ended, end bouncing
            if (bouncing && bounceBuffer <= 0)
            {
                movementEvent.SetIsBounceAnimating(false);
            }
        }
        #endregion

		// Jump checks
        #region JUMP CHECKS
		// If the Player is jumping and the RB.velocity is downward, they are falling
        if (isJumping && RB.velocity.y < 0)
        {
            isJumping = false;

            isJumpFalling = true;

            currentState = PlayerState.Falling;
            movementEvent.SetCurrentState(currentState);

            movementEvent.Fall();
        }

		// If the plaer is not Jumping and the time from when they were last on the ground is greater than 0,
		// they are not jumpCutting
        if (lastOnGroundTime > 0 && !isJumping)
        {
            isJumpCut = false;

			// Double check that if they are not jumping, they are falling  
            if (!isJumping)
			{
                isJumpFalling = false;
            }
        }

		// If the player velocity is downward and the player is not touching the ground,
		// they are falling
        if (RB.velocity.y < 0 && !isGrounded)
        {
			// Allow for fast fall
			isJumpCut = false;

            isJumpFalling = true;

            currentState = PlayerState.Falling;
            movementEvent.SetCurrentState(currentState);

            movementEvent.Fall();
        }

        // Set Animations
        #region JUMP ANIMATION CHECKS
        // Set jumpAnimFalling to false so there can be a little animation buffer without affecting the actual movement
        isJumpAnimFalling = false;

        // If the player is Jumping, update variables
        if (isJumping)
        {
            isGrounded = false;
            movementEvent.Jump();
        }

		// If the player is falling or their velocity downwards is greater than -0.1,
		// update variables and set the falling animation
        if ((isJumpFalling || RB.velocity.y < -0.1))
        {
			if(!isOnSlope)
			{
                isGrounded = false;

                // if the falling buffer is true,
                if (fallingBuffer <= 0)
				{
                    isJumpAnimFalling = true;
                    movementEvent.Fall();
                }
            }
        }
        else if (!isJumpFalling || isGrounded || bouncing || isOnSlope) // Otherwise, if the player is not falling, update animations
        {
            fallingBuffer = 0.15f;
        }
		
		if(bouncing && !(RB.velocity.y <= 0f)) // Also check for when bouncing is true
		{
            fallingBuffer = 0.15f;
        }
        #endregion

        // If the player can jump and the last pressed jump time is less than 0,
		// update variables and call the JumpFunction()
        if (CanJump() && lastPressedJumpTime > 0)
        {
            isJumping = true;
            isJumpCut = false;
            isJumpFalling = false;
            currentState = PlayerState.Jumping;
            movementEvent.SetCurrentState(currentState);
            Jump();
        }
        #endregion

		// Land Animation Checks
        #region LAND ANIMATION CHECKS
        // If the player has been on the ground for longer than 0 seconds, they have landed
        if (landedTimer > 0 && isGrounded && !bouncing && !isJumping && (previousState == PlayerState.Falling))
        {
            // Update timer
            landedTimer -= Time.deltaTime;

            // Set landed to true
            landed = true;

            // Trigger landing events
            movementEvent.Land();
        }
        else
        {
            // Set landed to false
            landed = false;
        }

		if(!isGrounded && RB.velocity.y < 0)
		{
			// If not grounded and has a negative velocity, reset landed timer
			landedTimer = 0.01f;
		}
        #endregion

        // Calculate Gravity
        #region GRAVITY
        if (!bouncing)
        {
            // Check for slope gravity first
            if (isOnSlope && !isLocked && !isJumping && !touchingShroom && canWalkOnSlope)
            {
                // Check for movement input
                if (moveInput.x == 0.0f)
                {
                    // If not moving, set to 0 so the player can stop on the hill
                    SetGravityScale(0);
                } else if (isGrounded && canWalkOnSlope)
                {
                    // If moving, apply normal gravity
                    SetGravityScale(data.gravityScale);
                }
            }
            else if (RB.velocity.y < 0 && moveInput.y < 0) // If fast falling
			{
				// Higher gravity if we've released the jump input or are falling
				// Much higher gravity if holding down
				SetGravityScale(data.gravityScale * data.fastFallGravityMult);

				// Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
				RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -data.maxFastFallSpeed));

                // Check if we need to hide fast fall message
                if (movementEvent.GetShowingFastFall() && movementEvent.GetTouchingFastFall())
                {
                    movementEvent.HideFastFallMessage();
                }
            }
			else if (isJumpCut) // If jump cutting
			{
				// Higher gravity if jump button released
				SetGravityScale(data.gravityScale * data.jumpCutGravityMult);
				RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -data.maxFallSpeed));
			}
			else if ((isJumping || isJumpFalling) && Mathf.Abs(RB.velocity.y) < data.jumpHangTimeThreshold) // If jump hanging
			{
				SetGravityScale(data.gravityScale * data.jumpHangGravityMult);
			}
            else if (RB.velocity.y < 0 && movementEvent.GetIsTouchingWall()) // If sliding down a wall
            {
                Debug.Log("Downward Wall Grav");
                // Lower gravity if sliding on a wall
                SetGravityScale(data.gravityScale * data.wallSlideGravityMultDown);

                // Caps maximum slide speed
                RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -data.maxWallSlideSpeed));
            }
            else if (RB.velocity.y < 0) // If regular falling
			{
				// Higher gravity if falling
				SetGravityScale(data.gravityScale * data.fallGravityMult);

				// Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
				RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -data.maxFallSpeed));
			} else 
            {
                // Default gravity if standing on a platform or moving upwards
                SetGravityScale(data.gravityScale);
            }
        }
        else
        {
            // If bouncing upwards, using bounceGravity
            if (RB.velocity.y > 0)
            {
                if(movementEvent.GetIsTouchingWall()) // Check if touching a wall
                {
                    Debug.Log("Upward Wall Grav");
                    SetGravityScale(data.gravityScale * data.wallSlideGravityMultBounceUp);
                } else
                {
                    // Higher gravity if falling
                    SetGravityScale(data.gravityScale * data.bounceGravityMult);

                    // Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                    RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -data.maxFallSpeed));
                }
			}
			else if (RB.velocity.y < 0 && moveInput.y < 0) // If fast falling from bounce
			{
				// Higher gravity if falling
				SetGravityScale(data.gravityScale * data.fastFallGravityMult);

				// Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
				RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -data.maxFastFallSpeed));

                // Check if we need to hide fast fall message
                if (movementEvent.GetShowingFastFall() && movementEvent.GetTouchingFastFall())
                {
                    movementEvent.HideFastFallMessage();
                }
            } 
            else if(RB.velocity.y < 0 && movementEvent.GetIsTouchingWall())
            {
                Debug.Log("Boucne Upward Wall Grav");
                // Lower gravity if sliding on a wall
                SetGravityScale(data.gravityScale * data.wallSlideGravityMultDown);

                // Caps maximum slide speed
                RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -data.maxWallSlideSpeed));
            }
			else if (RB.velocity.y < 0) // If falling from a bounce, use fallFromBounceGravity
            {
                // Higher gravity if falling
                SetGravityScale(data.gravityScale * data.fallFromBounceGravityMult);

                // Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -data.maxFallSpeed));
            }
        }
        #endregion

        // Sets grounded to false if you are bouncing 
        if (bouncing)
        {
            isGrounded = false;
        }

        // Check for the first frame for when the player is not touching a wall
        if (previousWallState && (previousWallState != movementEvent.GetIsTouchingWall()))
        {
            wallStickTimer = wallStickTimerMax;
        }

        movementEvent.SetIsGrounded(isGrounded);
        movementEvent.SetIsJumping(isJumping);
        movementEvent.SetIsFalling(isJumpAnimFalling);
        movementEvent.SetIsBouncing(bouncing);
        movementEvent.SetIsLanding(landed);

        // Update previousPlayerPosition for future calculations
        previousPlayerPosition = playerPosition;

        // Update previous state
        previousState = currentState;
        movementEvent.SetPreviousState(previousState);
        previousWallState = movementEvent.GetIsTouchingWall();
    }

	private void FixedUpdate()
	{
		// If the player isn't locked
		if (!isLocked)
		{
            // Check for slopes
			if(checkForSlopes)
			{
                // Handle slopes
                HandleSlopes();
            }

			// Handle movement
            if(canInput)
            {
                Run(0.5f);
                movementEvent.SetVectors(rb.velocity, moveInput);
                movementEvent.Move();

                // Set current state to running if the player is moving while grounded
                if(isGrounded)
                {
                    currentState = PlayerState.Running;
                    movementEvent.SetCurrentState(currentState);
                }

                // Check direction to face based on vector
                if (moveInput.x != 0 && movementEvent.GetCanTurn() && !movementEvent.GetIsTouchingWall())
                {
                    // Check directions to face
                    CheckDirectionToFace(moveInput.x > 0);
                }

                // If the player is not in the load trigger, set the levelpos type to default
                if (!loadLevelEvent.GetInLoadTrigger())
                {
                    levelDataObj.SetLevelPos(SceneManager.GetActiveScene().name, LEVELPOS.DEFAULT);
                }

                StopCoroutine(MovementCooldown());
            } else if(!cutsceneEvent.GetPlayingCutscene()) // Only apply the movement cooldown when playing the game and not in a cutscene
            {
                StartCoroutine(MovementCooldown());
            }

            // Check if the player is touching the wall
            if (movementEvent.GetIsTouchingWall())
            {
                if(!isGrounded)
                {
                    // Turn towards the wall
                    // TurnToWall();

                    // Trigger other wall-related events
                    movementEvent.Wall();
                }

                // Apply wall force
                ApplyWallForce();
            }
            else if (wallStickTimer > 0) // Check if the wall stick timer is running
            {
                // Apply wall force
                ApplyWallForce();
            }
        }
		else
		{
			// Reset velocity to 0
			if (RB.velocity.x != 0)
			{
				// If the player is moving rightward, you must subtract
				if (RB.velocity.x > 0)
				{
					RB.velocity -= Vector2.right * rb.velocity.x;
				}
				else if (RB.velocity.x < 0) // If the plaer is moving leftward, you must add
				{
					RB.velocity += Vector2.left * rb.velocity.x;
				}
			}
		}
    }

    #region INPUT CALLBACKS
    /// <summary>
    /// Update lastPressedJumpTime according to PlayerData
    /// </summary>
    public void OnJumpInput()
	{
		lastPressedJumpTime = data.jumpInputBufferTime;
	}

	/// <summary>
	/// Check if the Player can JumpCut when Jumping
	/// </summary>
	public void OnJumpUpInput()
	{
		if (CanJumpCut())
        {
			isJumpCut = true;
		}
	}
	#endregion

	#region GENERAL METHODS
	/// <summary>
	/// Set the Player's RigidBody's Gravity Scale
	/// </summary>
	/// <param name="scale">The Gravity Scale to set it to</param>
	public void SetGravityScale(float scale)
	{
		RB.gravityScale = scale;
	}
	#endregion

	// MOVEMENT METHODS
	#region RUN METHODS
	/// <summary>
	/// Allow the Player to Run
	/// </summary>
	/// <param name="lerpAmount">The amount to sooth movement by</param>
	private void Run(float lerpAmount)
	{
        // Calculate the direction we want to move in and our desired velocity
        float targetSpeed = moveInput.x * data.runMaxSpeed;

        // Reduce our control using Lerp() this smooths changes to are direction and speed
        targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);

        #region Calculate AccelRate
        float accelRate;

        // Set specific air accelerations and deccelerations for bouncing
        if(bouncing)
        {
            data.accelInAir = 0.75f;
            data.deccelInAir = 0f;
        } else
        {
            data.accelInAir = 0.75f;
            data.deccelInAir = 0.75f;
        }

        // Gets an acceleration value based on if we are accelerating (includes turning) 
        // or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
        if (lastOnGroundTime > 0 && !bouncing)
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? data.runAccelAmount : data.runDeccelAmount;
        }	
        else 
        {
            accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? data.runAccelAmount * data.accelInAir : data.runDeccelAmount * data.deccelInAir;
        }
        #endregion

        #region Add Bonus Jump Apex Acceleration
        // Increase our acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
        if ((isJumping || isJumpFalling) && Mathf.Abs(RB.velocity.y) < data.jumpHangTimeThreshold)
        {
            accelRate *= data.jumpHangAccelerationMult;
            targetSpeed *= data.jumpHangMaxSpeedMult;
        }
        #endregion

        #region Conserve Momentum
        // We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
        if (data.doConserveMomentum && Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && lastOnGroundTime < 0)
        {
            // Prevent any deceleration from happening, or in other words conserve our current momentum
            if (!bouncing)
            {
				accelRate = 0;
            }
            else
            {
                // If bouncing, add some air deceleration for consistency
                accelRate = 5;
            }
        }
		#endregion

		// Calculate difference between current velocity and desired velocity
		float speedDif = targetSpeed - RB.velocity.x;

        // Calculate force along x-axis to apply to thr player
        float movement = speedDif * accelRate;

        // Convert this to a vector and apply to rigidbody
        RB.AddForce(movement * Vector2.right, ForceMode2D.Force);

		// DEV TOOL
		// Check if noClip is on
        if (devTools.NoClip || settings.NoClipOn)
        {
			// Check if up/down input is detected
			if (moveInput.y != 0)
			{
				// Move player up/down
				GetComponent<Transform>().position += Vector3.up * moveInput.y;
			}
            // Check if right/left input is detected
            if (moveInput.x != 0)
            {
				// Move player left/right
                GetComponent<Transform>().position += Vector3.right * moveInput.x;
            }
        }
    }

    /// <summary>
    /// Force Player movement to stick on slopes
    /// </summary>
    private void HandleSlopes()
    {
        // Perform a slope check using a small ground detector circle
        Vector2 checkPos = playerCollider.bounds.center - (Vector3)(new Vector2(0.0f, capsuleColliderSize.y / 2));

        #region Check Horizontal Slope
        // Cast a front and back ray for nearby slopes and valleys
        RaycastHit2D frontHit = Physics2D.Raycast(checkPos, transform.right, slopeCheckDistance, groundLayer);
        RaycastHit2D backHit = Physics2D.Raycast(checkPos, -transform.right, slopeCheckDistance, groundLayer);

        // If either the front or back hit, set the angle and set isOnSlope to true
        if (frontHit)
        {
            slopeSideAngle = Vector2.Angle(frontHit.normal, Vector2.up);
            isOnSlope = true;
        }
        else if (backHit)
        {
            slopeSideAngle = Vector2.Angle(backHit.normal, Vector2.up);
            isOnSlope = true;
        }
        else
        {
            // If neither hit, then set the slopeSideAngle to 0 and isOnSlope to false
            slopeSideAngle = 0.0f;
            isOnSlope = false;
        }
        #endregion

        #region Check Vertical Slope
        // Cast a ray downward and chcck for collisions with the ground
        RaycastHit2D downHit = Physics2D.Raycast(checkPos, Vector2.down, slopeCheckDistance, groundLayer);

        // If the ray hits the ground
        if (downHit)
        {
            // Create a normal vector from the collision point between the ray and the ground
            slopeNormal = downHit.normal.normalized;

            // Get an angle from the slope normal using trig
            slopeDownAngle = Mathf.Atan2(slopeNormal.y, slopeNormal.x) * Mathf.Rad2Deg;

            // Create a perpendicular vector from the slope normal and normalize it
            slopeNormalPerp = Vector2.Perpendicular(slopeNormal).normalized;

            // Get the angle from the perpendicular vector using trig
            slopeNormalPerpAngle = Mathf.Atan2(slopeNormalPerp.y, slopeNormalPerp.x) * Mathf.Rad2Deg;

            // If the angle is not close to flat (180 degrees), then you're on a slope
            if (!(Mathf.Abs(slopeNormalPerpAngle) >= 179f && Mathf.Abs(slopeNormalPerpAngle) <= 181f))
            {
                isOnSlope = true;
            }
            else
            {
                // If it is near-flat (180 degrees), then you're not on a slope
                isOnSlope = false;
            }

            // Draw a ray for debugging
            Debug.DrawRay(downHit.point, downHit.normal, Color.red);
        }
        else
        {
            // If there is not hit, then set isOnSlope to false by default
            isOnSlope = false;
        }
        #endregion

        #region Barrier Checks
        // If the slope angle is less than the max slope that the player can climb, then allow them to walk on it, as long as they're grounded
        if (Mathf.Abs(slopeDownAngle) < maxSlopeAngle || slopeSideAngle < maxSlopeAngle && isGrounded)
        {
            canWalkOnSlope = true;
        }
        else
        {
            // Otherwise, do not allow them to walk on it
            canWalkOnSlope = false;
        }

        currentSlopeDownAngle = Mathf.Abs(slopeDownAngle);
        currentSlopeSideAngle = slopeSideAngle;

        #endregion

        // If the player is grounded, is on a slope, is able to walk on the slope, and is not jumping, and is not bouncing, then apply the slope force
        if (isGrounded && isOnSlope && canWalkOnSlope && !isJumping && !bouncing && !touchingShroom)
        {
            Vector2 slopeForce = -slopeNormal * slopeForceMagnitude;
            rb.AddForce(slopeForce, ForceMode2D.Force);

            // Draw for debugging
            Debug.DrawRay(checkPos, slopeForce, Color.white);
        }
        else
        {
            // If not on a slope, apply the identity Quaternion to get the player back to normal rotation
            spriteRenderer.gameObject.transform.rotation = Quaternion.identity;
        }

        // Set frictions based on still input so that the player doesn't move - not sure if this actually does anything as the gravity
        // takes care of most of it
        if (isOnSlope && canWalkOnSlope && moveInput.x == 0.0f)
        {
            rb.sharedMaterial = fullFriction;
            playerCollider.sharedMaterial = fullFriction;
        }
        else
        {
            rb.sharedMaterial = noFriction;
            playerCollider.sharedMaterial = noFriction;
        }

        // Draw rays for debugging
        if(debug)
        {
            Debug.DrawRay(checkPos, new Vector3(0, -slopeCheckDistance, 0), Color.cyan); // Downward distance check
            Debug.DrawRay(checkPos, new Vector3(slopeCheckDistance, 0, 0), Color.blue); // Right distance check
            Debug.DrawRay(checkPos, new Vector3(-slopeCheckDistance, 0, 0), Color.yellow); // Left distance check
        }
    }

    /// <summary>
    /// Add forces to player movement to be stickier towards walls
    /// </summary>
    private void ApplyWallForce()
    {
        // Get the vector from the center of the playerp position to the wall
        Vector2 checkPos = playerCollider.bounds.center;
        Vector2 wallDir = new Vector2(movementEvent.GetWallCollisionPoint().x, checkPos.y);

        // Calculate the stick vector
        Vector2 stickVector = wallDir - checkPos;
        Vector2 stickForce = stickVector.normalized * wallStickForce;

        // Add the force
        RB.AddForce(stickForce, ForceMode2D.Force);

        // Draw rays for debugging
        if (debug)
        {
            Debug.DrawRay(transform.position, stickForce, Color.magenta);
        }
    }

    private void TurnToWall()
    {
        // Get the vector from the center of the playerp position to the wall
        Vector2 checkPos = playerCollider.bounds.center;
        Vector2 wallDir = new Vector2(movementEvent.GetWallCollisionPoint().x, checkPos.y);

        // Calculate the stick vector
        Vector2 stickVector = wallDir - checkPos;

        // Turn the opposite way
        Turn((int)-stickVector.normalized.x);
    }

    /// <summary>
    /// Turn the Player to face the direction they are moving in
    /// </summary>
    public void Turn()
	{
        // Stores scale and flips the player along the x axis, 
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        isFacingRight = !isFacingRight;
	}

    /// <summary>
    /// Turn the player to face a certain direction
    /// </summary>
    /// <param name="direction">The direction to face, -1 for left, 1 for right</param>
    public void Turn(int direction)
    {
        // Get the localScale of the player
        Vector3 scale = transform.localScale;

        // Change scale depending on parameters
        switch(direction)
        {
            case -1:
                // If the scale is positive, turn it negative
                if(scale.x > 0)
                {
                    scale.x *= -1f;
                }
                isFacingRight = false;
                break;

            case 1:
                // Turn the scale positive
                scale.x = Mathf.Abs(scale.x);
                isFacingRight = true;
                break;

            // Fallthrough case
            default:
                break;
        }

        // Apply turn
        transform.localScale = scale;
    }

    /// <summary>
    /// Reset the way the player is looking
    /// </summary>
    public void ResetTurn()
    {
        // Resets the scale
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x);
        transform.localScale = scale;

        isFacingRight = true;
    }
	#endregion

    // JUMP METHODS
	#region JUMP METHODS
	/// <summary>
	/// Allow the Player to Jump
	/// </summary>
	private void Jump()
	{
		// Ensures we can't call Jump multiple times from one press
		lastPressedJumpTime = 0;
		lastOnGroundTime = 0;

        #region Perform Jump
        // If the player is grounded and moving upwards, force velocity to 0
        // so jumps stay consistent
        if (isGrounded && RB.velocity.y > 0)
        {
            RB.velocity -= Vector2.up * rb.velocity.y;
        }

        // We increase the force applied if we are falling
        // This means we'll always feel like we jump the same amount
        float force = data.jumpForce;
        if (RB.velocity.y < 0)
        {
            force -= RB.velocity.y;
        }

        // Add the force to the Player's RigidBody
        RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        createDustOnFall = true;
        #endregion
    }
    #endregion

    // CHECK METHODS
    #region CHECK METHODS
    /// <summary>
    /// Check which direction the Player is facing
    /// </summary>
    /// <param name="isMovingRight">A boolean representing which direction the Player is moving</param>
    public void CheckDirectionToFace(bool isMovingRight)
	{
        if (isMovingRight != isFacingRight)
		{
			Turn();
		}
	}

    public void CheckForWall()
    {
        RaycastHit2D wallCheckRight = Physics2D.BoxCast(GameObject.Find("PlayerSprite").GetComponent<BoxCollider2D>().bounds.center, new Vector3(playerCollider.bounds.size.x - 0.35f, playerCollider.bounds.size.y, playerCollider.bounds.size.z), 0f, Vector2.left, 0.25f, wallLayer);
        RaycastHit2D wallCheckLeft = Physics2D.BoxCast(GameObject.Find("PlayerSprite").GetComponent<BoxCollider2D>().bounds.center, new Vector3(playerCollider.bounds.size.x + 0.35f, playerCollider.bounds.size.y, playerCollider.bounds.size.z), 0f, Vector2.right, 0.25f, wallLayer);

        if(wallCheckRight)
        {
            movementEvent.SetIsTouchingWall(true);
            movementEvent.SetWallSide(P_WALLCHECK.RIGHT);
        } else if(wallCheckLeft)
        {
            movementEvent.SetIsTouchingWall(true);
            movementEvent.SetWallSide(P_WALLCHECK.LEFT);
        } else
        {
            movementEvent.SetIsTouchingWall(false);
            movementEvent.SetWallSide(P_WALLCHECK.NONE);
        }
    }

	/// <summary>
	/// Check if the Player can Jump
	/// </summary>
	/// <returns>A boolean that states whether the Player can Jump or not</returns>
	private bool CanJump()
	{
		return lastOnGroundTime > 0 && !isJumping && !isLocked;
	}

	/// <summary>
	/// Check if the Player can Jump Cut, or Fast Fall
	/// </summary>
	/// <returns>A boolean that states whether the Player can Jump Cut</returns>
	private bool CanJumpCut()
	{
		return isJumping && RB.velocity.y > 0;
	}
    #endregion

    // COROUTINES
    #region COROUTINES
    /// <summary>
    /// Apply a movement cooldown
    /// </summary>
    /// <returns></returns>
    private IEnumerator MovementCooldown()
    {
        if(inputCooldown > 0f)
        {
            yield return null;

            inputCooldown -= Time.deltaTime;
        } else
        {
            inputCooldown = 0.05f;
            canInput = true;
            yield return null;
        }
    }

    /// <summary>
    /// Set a jump buffer for jump-bouncing
    /// </summary>
    /// <returns></returns>
    private IEnumerator JumpBuffer()
    {
        // Set the buffer
        if(jumpBuffer <= 0f)
        {
            jumpBuffer = 0.1f;
        }

        // While the buffer is greater than 0, let other code run
        // then subtract by deltaTime
        while(jumpBuffer > 0f)
        {
            yield return null;

            jumpBuffer -= Time.deltaTime;
        }

        yield return null;
    }
    #endregion

    // INPUT HANDLER
    #region INPUT HANDLER
    /// <summary>
    /// Activate Player movement using controls
    /// </summary>
    /// <param name="context">The context of the Controller being used</param>
    public void OnMove(InputAction.CallbackContext context)
    {
		// Check if the game is paused
        if (!isLocked && canInputHard)
        {
			// Set the move input to the value returned by context
			moveInput = context.ReadValue<Vector2>();
		}
	}

	/// <summary>
	/// Activate Player jump using controls
	/// </summary>
	/// <param name="context">The context of the Controller being used</param>
	public void OnJump(InputAction.CallbackContext context)
	{
		// Check if the game is paused
        if (!isLocked && !bouncing && !touchingShroom && canInputHard)
        {
			// Check jump based on whether the bind was pressed or released
			if (context.started)
			{
				OnJumpInput();
                StartCoroutine(JumpBuffer());
            }
			else if (context.canceled)
			{
				OnJumpUpInput();
			}
		}
    }
	#endregion

    // EVENT FUNCTIONS
	#region EVENT FUNCTIONS
	/// <summary>
	/// Set variables for bouncing
	/// </summary>
	private void ApplyBounce(Vector3 bounceForce, ForceMode2D forceType)
	{
        bouncing = true;
        bounceBuffer = 0.1f;
        landedTimer = 0.2f;
        currentState = PlayerState.Bouncing;
        movementEvent.SetCurrentState(currentState);

        // Check if jumping - if there's a simultaneous jump,
        // reduce the bounce amount so that the player doesn't launch into the air
        // more than they are supposed to
        if (isJumping && jumpBuffer > 0 && !movementEvent.GetIsTouchingWall())
        {
            bounceForce /= data.jumpForce;
        }

        // Add forces
        //mushroomEvent.SetBounceForce(bounceForce);
        RB.AddForce(bounceForce, forceType);

        // Play the shroom sound
        AudioManager.Instance.PlayRandomizedOneShot(FMODEvents.Instance.ShroomBounces, transform.position);
    }

	/// <summary>
	/// Set whether the player is touching a shroom
	/// </summary>
	/// <param name="touchingData"></param>

	private void TouchingShroom(bool touchingData)
	{
		touchingShroom = touchingData;
	}

    private void SetLoadPos()
    {
        transform.position = DataManager.Instance.Data.levelData[SceneManager.GetActiveScene().name].playerPosition;
    }

    /// <summary>
    /// Lock player movement
    /// </summary>
    /// <param name="lockedData"></param>
    private void LockMovement()
	{
        moveInput = Vector2.zero;
        movementEvent.SetCanTurn(false);
        isLocked = true;
	}

    /// <summary>
    /// Unlock player movement
    /// </summary>
    private void UnlockMovement()
    {
        isLocked = false;

        if (!playerInput.inputIsActive)
        {
            playerInput.ActivateInput();
        }
        movementEvent.SetCanTurn(true);
    }

    /// <summary>
    /// Stop input
    /// </summary>
    /// <param name="cooldownData">The amount of time to stop the input for</param>
    private void StopInput(float cooldownData)
    {
        canInput = false;
        inputCooldown = cooldownData;
    }

    /// <summary>
    /// Disable player movement input
    /// </summary>
    private void DisableInput()
    {
        playerInput.DeactivateInput();
        movementEvent.SetCanTurn(false);
        canInput = false;
        canInputHard = false;
    }

    /// <summary>
    /// Enable player movement input
    /// </summary>
    private void EnableInput()
    {
        playerInput.ActivateInput();
        movementEvent.SetCanTurn(true);
        canInput = true;
        canInputHard = true;
    }

    /// <summary>
    /// Set the player movement direction
    /// </summary>
    /// <param name="movementDirection">The movement direction</param>
    private void SetMovementDirection(Vector2 movementDirection)
    {
        RB.velocity = movementDirection;
    }

    /// <summary>
    /// Set in which direction the player is facing
    /// </summary>
    /// <param name="directionToface">The direction to face, 1 for right, -1 for left</param>
    private void SetTurnDirection(int directionToFace)
    {
        switch(directionToFace)
        {
            // If directionToFace is 1, set the player facing right
            case 1:
                Vector3 scaleRight = transform.localScale;
                scaleRight.x = Mathf.Abs(scaleRight.x);
                transform.localScale = scaleRight;

                isFacingRight = true;
                break;

            // If directionToFace is -1, set the player facing left
            case -1:
                Vector3 scaleLeft = transform.localScale;
                scaleLeft.x = Mathf.Abs(scaleLeft.x);
                scaleLeft.x = -scaleLeft.x;
                transform.localScale = scaleLeft;

                isFacingRight = false;
                break;
        }
    }

    private void Land()
    {

    }
	#endregion

	// DATA HANDLING
	#region DATA HANDLING
	public void LoadData(GameData data)
	{
        // Load level data
        levelDataObj.LoadData(data);

        // Get the currently active scene
        Scene scene = SceneManager.GetActiveScene();

        // Check if that scene name exists in the dictionary for good measure
        if(data.levelData.ContainsKey(scene.name))
        {
            // If it does exist, load the players positional data using the data for this scene
            Vector3 playerPositionForThisScene = data.levelData[scene.name].playerPosition;
            transform.position = playerPositionForThisScene;
        } else
        {
            //If it doesn't exist, let ourselves know that we need to add it to our game data
            Debug.LogError("Failed to get data for scene with name: " + scene.name + ". It may need to be added to the GameData constructor");
        }

        // Load movement data
        movementEvent.LoadData(data);
	}

	public void SaveData(GameData data)
	{
        // Save player position in the dictionary slot for this scene
        Scene scene = SceneManager.GetActiveScene();
        data.levelData[scene.name].playerPosition = transform.position;

        // Save movement and level data
        movementEvent.SaveData(data);
        levelDataObj.SaveData(data);
	}
    #endregion
}
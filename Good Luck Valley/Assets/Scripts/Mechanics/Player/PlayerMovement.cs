using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region REFERENCES
    public PlayerData Data;
    public GameObject playerLight;
    public Rigidbody2D RB { get; private set; }
    public Animator animator;
    public BouncingEffect bounceEffect;
    private PauseMenu pauseMenu;
    private CompositeCollider2D mapCollider;
    public Transform groundCheckPoint;
    [SerializeField] private LayerMask groundLayer;
    #endregion

    #region FIELDS
    private bool isMoving;
	private bool isJumping;
    private bool isJumpCut;
	private bool isJumpFalling;
	private bool isFacingRight;
    private bool isGrounded;
    private bool inputHorizontal;
    private bool isLocked = false;
    [SerializeField] private bool justLanded = false;
	private float lastOnGroundTime;
	private float lastPressedJumpTime;
    [SerializeField] float landedTimer = 0f;
    private Vector2 playerPosition;
    private Vector2 previousPlayerPosition;
    private Vector2 distanceFromLastPosition;
    [SerializeField] private Vector2 moveInput;
    public Vector2 groundCheckSize = new Vector2(0.49f, 0.03f);
	#endregion

	#region PROPERTIES
	public bool IsMoving { get { return isMoving; } set { isMoving = value; } }
    public bool IsJumping { get { return isJumping; } set { isJumping = value; } }
    public bool IsFacingRight { get { return isFacingRight; } set { isFacingRight = value; } }
	public bool IsGrounded { get { return isGrounded; } set { isGrounded = value; } }
    public bool InputHorizontal { get { return inputHorizontal; } set { inputHorizontal = value; } }
	public bool IsLocked { get { return isLocked; } set { isLocked = value; } }
	public Vector2 DistanceFromLastPosition { get { return distanceFromLastPosition; } set { distanceFromLastPosition = value; } }
    public Vector2 MoveInput { get { return moveInput; } set { moveInput = value; } }
    #endregion

    private void Awake()
	{
		RB = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		bounceEffect = GetComponent<BouncingEffect>();
		pauseMenu = GameObject.Find("PauseUI").GetComponent<PauseMenu>();
		mapCollider = GameObject.Find("foreground").GetComponent<CompositeCollider2D>();
	}

	private void Start()
	{
		SetGravityScale(Data.gravityScale);
		isFacingRight = true;
		playerPosition = transform.position;
		playerLight = GameObject.Find("PlayerLight");
	}

	private void Update()
	{
		// Set playerPosition to the current position and calculate the distance from the previous position
        playerPosition = transform.position;
        distanceFromLastPosition = playerPosition - previousPlayerPosition;

		// Check if the player is moving using RB.velocity
        isMoving = false;
        if (RB.velocity != Vector2.zero)
        {
            isMoving = true;
        }

		// Set the playerLight's position to the player's position
        playerLight.transform.position = transform.position;

		// Update timers
        #region TIMERS
        lastOnGroundTime -= Time.deltaTime;

        lastPressedJumpTime -= Time.deltaTime;
        #endregion

		// Check for Collisions
        #region COLLISION CHECKS
        if (!isJumping)
        {
			// Ground Check
			if (Physics2D.OverlapBox(groundCheckPoint.position, groundCheckSize, 0, groundLayer) && !isJumping) // Checks if set box overlaps with ground
			{
				// If bouncing beore, end bouncing
				if (bounceEffect.Bouncing)
				{
					bounceEffect.Bouncing = false;
				}

				// Ground player
				isGrounded = true;

				// Set coyote time
				lastOnGroundTime = Data.coyoteTime;

				// If the player has been on the ground for longer than 0 seconds, they have landed
				if (landedTimer > 0)
				{
					// Update variables and set animations
					landedTimer -= Time.deltaTime;
					justLanded = true;
					animator.SetBool("JustLanded", true);
				}
				else
				{
					// Otherwise, they have not landed - update
					// variables and set animations
					justLanded = false;
					animator.SetBool("JustLanded", false);
				}
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
        }

        // Set Animations
        #region JUMP ANIMATION CHECKS
        // If the player is Jumping, update variables and set the jump animation
        if (isJumping)
        {
            isGrounded = false;
            animator.SetBool("Jump", true);
        }
        else if (!isJumping) // Else, if the player is not jumping, update animations
        {
            animator.SetBool("Jump", false);
        }

		// If the player is falling or their velocity downwards is greater than -0.1,
		// update variables and set the falling animation
        if (isJumpFalling || RB.velocity.y < -0.1)
        {
            isGrounded = false;
            animator.SetBool("Falling", true);
        }
        else if (!isJumpFalling) // Otherwise, if the player is not falling, update animations
        {
            animator.SetBool("Falling", false);
        }
        #endregion

        // If the player can jump and the last pressed jump time is less than 0,
		// update variables and call the JumpFunction()
        if (CanJump() && lastPressedJumpTime > 0)
        {
            isJumping = true;
            isJumpCut = false;
            isJumpFalling = false;
            Jump();
        }
        #endregion

		// Calculate Gravity
        #region GRAVITY
        if (!bounceEffect.Bouncing)
        {
            // Higher gravity if we've released the jump input or are falling
            if (RB.velocity.y < 0 && moveInput.y < 0)
            {
                // Much higher gravity if holding down
                SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);

                // Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFastFallSpeed));
            }
            else if (isJumpCut)
            {
                // Higher gravity if jump button released
                SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
                RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
            }
            else if ((isJumping || isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
            {
                SetGravityScale(Data.gravityScale * Data.jumpHangGravityMult);
            }
            else if (RB.velocity.y < 0)
            {
                // Higher gravity if falling
                SetGravityScale(Data.gravityScale * Data.fallGravityMult);

                // Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
            }
            else
            {
                // Default gravity if standing on a platform or moving upwards
                SetGravityScale(Data.gravityScale);
            }
        }
        else
        {
            // If bouncing upwards, using bounceGravity
            if (RB.velocity.y > 0)
            {
                // Higher gravity if falling
                SetGravityScale(Data.gravityScale * Data.bounceGravityMult);

                // Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
            }
            else if (RB.velocity.y < 0) // If falling from a bounce, use fallFromBounceGravity
            {
                // Higher gravity if falling
                SetGravityScale(Data.gravityScale * Data.fallFromBounceGravityMult);

                // Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
            }
        }
        #endregion

		// Update previousPlayerPosition for future calculations
        previousPlayerPosition = playerPosition;
    }

    private void FixedUpdate()
	{
        // Handle Run
        Run(1);

		// Set animation
        animator.SetFloat("Speed", Mathf.Abs(moveInput.x));
    }

	#region INPUT CALLBACKS
	/// <summary>
	/// Update lastPressedJumpTime according to PlayerData
	/// </summary>
	public void OnJumpInput()
	{
		lastPressedJumpTime = Data.jumpInputBufferTime;
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
		float targetSpeed = moveInput.x * Data.runMaxSpeed;

		// Reduce are control using Lerp() this smooths changes to are direction and speed
		targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);

		#region Calculate AccelRate
		float accelRate;

		// Gets an acceleration value based on if we are accelerating (includes turning) 
		// or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
		if (lastOnGroundTime > 0)
        {
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount : Data.runDeccelAmount;
		}
        else
        {
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? Data.runAccelAmount * Data.accelInAir : Data.runDeccelAmount * Data.deccelInAir;
		}
		#endregion

		#region Add Bonus Jump Apex Acceleration
		// Increase are acceleration and maxSpeed when at the apex of their jump, makes the jump feel a bit more bouncy, responsive and natural
		if ((isJumping || isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
		{
			accelRate *= Data.jumpHangAccelerationMult;
			targetSpeed *= Data.jumpHangMaxSpeedMult;
		}
		#endregion

		#region Conserve Momentum
		// We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
		if (Data.doConserveMomentum && Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && lastOnGroundTime < 0)
		{
			// Prevent any deceleration from happening, or in other words conserve are current momentum
			if(!bounceEffect.Bouncing)
            {
				accelRate = 0;
			} else
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
	#endregion

	#region JUMP METHODS
	/// <summary>
	/// Allow the Player to Jump
	/// </summary>
	private void Jump()
	{
		// Ensures we can't call Jump multiple times from one press
		lastPressedJumpTime = 0;
		lastOnGroundTime = 0;
		landedTimer = 0.2f;

		#region Perform Jump
		// We increase the force applied if we are falling
		// This means we'll always feel like we jump the same amount 
		float force = Data.jumpForce;
		if (RB.velocity.y < 0)
        {
			force -= RB.velocity.y;
		}

		// Add the force to the Player's RigidBody
		RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
		#endregion
	}
	#endregion

	#region CHECK METHODS
	/// <summary>
	/// Check which direction the Player is facing
	/// </summary>
	/// <param name="isMovingRight">A boolean representing which direction the Player is moving</param>
	public void CheckDirectionToFace(bool isMovingRight)
	{
		if (isMovingRight != isFacingRight)
			Turn();
	}

	/// <summary>
	/// Check if the Player can Jump
	/// </summary>
	/// <returns>A boolean that states whether the Player can Jump or not</returns>
	private bool CanJump()
	{
		return lastOnGroundTime > 0 && !isJumping;
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

	// INPUT HANDLER
	#region INPUT HANDLER
	/// <summary>
	/// Activate Player movement using controls
	/// </summary>
	/// <param name="context">The context of the Controller being used</param>
	public void OnMove(InputAction.CallbackContext context)
    {
		// Check if the game is paused
        if (!pauseMenu.Paused)
        {
			// Set the move input to the value returned by context
			moveInput = context.ReadValue<Vector2>();
			
			// Check direction to face based on vector
			if (moveInput.x != 0)
			{
				// Set inputHorizontal to true
                inputHorizontal = true;

				// Check directions to face
                CheckDirectionToFace(moveInput.x > 0);
			}

			// If the bind is no longer pressed, set inputHorizontal to false
			if (context.canceled)
			{
				inputHorizontal = false;
			}
		}
	}

	/// <summary>
	/// Activate Player jump using controls
	/// </summary>
	/// <param name="context">The context of the Controller being used</param>
	public void OnJump(InputAction.CallbackContext context)
	{
		// Check if the game is paused
        if (!pauseMenu.Paused)
        {
			// Check jump based on whether the bind was pressed or released
			if (context.started)
			{
				OnJumpInput();
			}
			else if (context.canceled)
			{
				OnJumpUpInput();
			}
		}
	}
	#endregion

	/// <summary>
	/// Show Gizmos regarding Player movement
	/// </summary>
	#region EDITOR METHODS
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckSize);
		Gizmos.color = Color.blue;
	}
	#endregion
}
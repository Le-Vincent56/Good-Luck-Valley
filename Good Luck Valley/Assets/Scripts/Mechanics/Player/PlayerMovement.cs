using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.Playables;
using UnityEngine.UIElements;
using UnityEngine.Windows;
using FMOD.Studio;

public class PlayerMovement : MonoBehaviour, IData
{
    #region REFERENCES
    [SerializeField] private PlayerData data;
    [SerializeField] private MovementScriptableObj movementEvent;
    [SerializeField] private MushroomScriptableObj mushroomEvent;
    [SerializeField] private DisableScriptableObj disableEvent;
	private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject playerLight;
	[SerializeField] private Rigidbody2D rb;
	private BoxCollider2D playerCollider;
	private CapsuleCollider2D capsuleCollider;
    [SerializeField] private LayerMask groundLayer;
	[SerializeField] private PhysicsMaterial2D noFriction;
	[SerializeField] private PhysicsMaterial2D fullFriction;
	private DevTools devTools;
	private Settings settings;
    #endregion

    #region FIELDS
    [SerializeField] bool debug = false;
    [SerializeField] bool isJumping;
    [SerializeField] private bool isGrounded;
    [SerializeField] private bool isLocked = false;
    [SerializeField] private bool canInput = true;
    [SerializeField] float fallingBuffer = 0.25f;
    [SerializeField] float landedTimer = 0f;
    [SerializeField] private float inputCooldown = 0.05f;
    private bool isMoving;
    private bool isJumpCut;
	private bool isJumpFalling;
    private bool isJumpAnimFalling;
	private bool isFacingRight;
    private bool landed;
    private float lastOnGroundTime;
    private float lastPressedJumpTime;
    private Vector2 playerPosition;
    private Vector2 previousPlayerPosition;
    private Vector2 distanceFromLastPosition;

    #region SLOPES
    [SerializeField] bool checkForSlopes = false;
    [SerializeField] bool isOnSlope;
    [SerializeField] bool canWalkOnSlope;
    [SerializeField] float slopeCheckDistance;
    [SerializeField] float slopeForceMagnitude = 5f;
    [SerializeField] float maxSlopeAngle;
    [SerializeField] private Vector2 moveInput;
    private Vector2 capsuleColliderSize;
	private Vector2 slopeNormal;
    private Vector3 slopeNormalPerp;
    private float slopeSideAngle;
	private float slopeDownAngle;
	private float slopeNormalPerpAngle;
	#endregion

	#region BOUNCING
	[SerializeField] private bool bouncing = false;
    [SerializeField] private bool touchingShroom = false;
    private float bounceBuffer = 0.1f;
	#endregion
	#endregion

	#region PROPERTIES
    public Rigidbody2D RB { get { return rb; } set { rb = value; } }
	public bool IsFacingRight { get { return isFacingRight; } set { isFacingRight = value; } }
    public bool IsMoving { get { return isMoving; } set { isMoving = value; } }
    public Vector2 DistanceFromLastPosition { get { return distanceFromLastPosition; } }
    #endregion

    private void Awake()
	{
		RB = GetComponent<Rigidbody2D>();
		spriteRenderer = GameObject.Find("PlayerSprite").GetComponent<SpriteRenderer>();
		playerCollider = GetComponent<BoxCollider2D>();
		capsuleCollider = GetComponent<CapsuleCollider2D>();
		devTools = GameObject.Find("Dev Tools").GetComponent<DevTools>();
		settings = GameObject.Find("MenusManager").GetComponent<Settings>();
	}

    private void OnEnable()
    {
        mushroomEvent.bounceEvent.AddListener(ApplyBounce);
        mushroomEvent.touchingShroomEvent.AddListener(TouchingShroom);
        disableEvent.pauseEvent.AddListener(LockMovement);
        disableEvent.unpauseEvent.AddListener(UnlockMovement);
        disableEvent.lockPlayerEvent.AddListener(LockMovement);
        disableEvent.unlockPlayerEvent.AddListener(UnlockMovement);
        disableEvent.stopInputEvent.AddListener(StopInput);
    }

    private void OnDisable()
    {
        mushroomEvent.bounceEvent.RemoveListener(ApplyBounce);
        mushroomEvent.touchingShroomEvent.RemoveListener(TouchingShroom);
        disableEvent.pauseEvent.RemoveListener(LockMovement);
        disableEvent.unpauseEvent.RemoveListener(UnlockMovement);
        disableEvent.lockPlayerEvent.RemoveListener(LockMovement);
        disableEvent.unlockPlayerEvent.RemoveListener(UnlockMovement);
        disableEvent.stopInputEvent.RemoveListener(StopInput);
    }

    private void Start()
	{
		SetGravityScale(data.gravityScale);
		isFacingRight = true;
		playerPosition = transform.position;
		playerLight = GameObject.Find("PlayerLight");
		capsuleColliderSize = capsuleCollider.size;
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

        // If the player is falling, update the fallingBuffer
        if (RB.velocity.y < 0 && !isGrounded)
        {
			fallingBuffer -= Time.deltaTime;
        }

        if (bounceBuffer > 0 && bouncing)
        {
            bounceBuffer -= Time.deltaTime;
        }
        #endregion

        // Check for Collisions
        #region COLLISION CHECKS
        if (!isJumping)
        {
			RaycastHit2D boxCheckGround = Physics2D.BoxCast(GameObject.Find("PlayerSprite").GetComponent<BoxCollider2D>().bounds.center, new Vector3(playerCollider.bounds.size.x - 0.1f, playerCollider.bounds.size.y, playerCollider.bounds.size.z), 0f, Vector2.down, 0.1f, groundLayer);

            if ((boxCheckGround || touchingShroom) && !isJumping) // Checks if set box overlaps with ground
            {
                // If bouncing before and the bounce buffer has ended, end bouncing
                if (bouncing && bounceBuffer <= 0)
                {
                    bouncing = false;
                    mushroomEvent.SetBounce(false);
                }

                // Ground player
                isGrounded = true;

                // Set coyote time
                lastOnGroundTime = data.coyoteTime;
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
        // Set jumpAnimFalling to false so there can be a little animation buffer without affecting the actual movement
        isJumpAnimFalling = false;

        // If the player is Jumping, update variables
        if (isJumping)
        {
            isGrounded = false;
        }

		// If the player is falling or their velocity downwards is greater than -0.1,
		// update variables and set the falling animation
        if ((isJumpFalling || RB.velocity.y < -0.1))
        {
			if(!isOnSlope)
			{
                isGrounded = false;

                // if the falling buffer is true,
				if(fallingBuffer <= 0)
				{
                    isJumpAnimFalling = true;
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
            Jump();
        }
        #endregion

		// Land Animation Checks
        #region LAND ANIMATION CHECKS
        // If the player has been on the ground for longer than 0 seconds, they have landed
        if (landedTimer > 0 && isGrounded && !bouncing)
        {
            // Update timer
            landedTimer -= Time.deltaTime;

            // Set landed to true
            landed = true;
        }
        else
        {
            // Set landed to false
            landed = false;
        }

		if(!isGrounded && RB.velocity.y < 0)
		{
			// If not grounded and has a negative velocity, reset landed timer
			landedTimer = 0.2f;
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
				if(moveInput.x == 0.0f)
				{
					// If not moving, set to 0 so the player can stop on the hill
                    SetGravityScale(0);
                } else if(isGrounded && canWalkOnSlope)
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
			}
			else if (isJumpCut)
			{
				// Higher gravity if jump button released
				SetGravityScale(data.gravityScale * data.jumpCutGravityMult);
				RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -data.maxFallSpeed));
			}
			else if ((isJumping || isJumpFalling) && Mathf.Abs(RB.velocity.y) < data.jumpHangTimeThreshold)
			{
				SetGravityScale(data.gravityScale * data.jumpHangGravityMult);
			}
			else if (RB.velocity.y < 0)
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
                // Higher gravity if falling
                SetGravityScale(data.gravityScale * data.bounceGravityMult);

                // Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
                RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -data.maxFallSpeed));
			}
			else if (RB.velocity.y < 0 && moveInput.y < 0) // If fast falling from bounce
			{
				// Higher gravity if falling
				SetGravityScale(data.gravityScale * data.fastFallGravityMult);

				// Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
				RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -data.maxFastFallSpeed));
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

        // Update previousPlayerPosition for future calculations
        previousPlayerPosition = playerPosition;
    }

	private void FixedUpdate()
	{
		// If the player isn't locked
		if (!isLocked)
		{
			if(checkForSlopes)
			{
                // Handle slopes
                HandleSlopes();
            }

			// Handle movement
            if(canInput)
            {
                Run(0.5f);
                StopCoroutine(MovementCooldown());
            } else
            {
                StartCoroutine(MovementCooldown());
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

        // Trigger events
        movementEvent.SetBools(isGrounded, isJumping, isJumpAnimFalling, landed);
        movementEvent.SetVectors(rb.velocity, moveInput);
        movementEvent.Move();
        movementEvent.Jump();
        movementEvent.Fall();
        movementEvent.Land();
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
        // If the slope angle is less than the max slope that the player can climb, then allow them to walk on it
        if (Mathf.Abs(slopeDownAngle) < maxSlopeAngle || slopeSideAngle < maxSlopeAngle)
        {
            canWalkOnSlope = true;
        }
        else
        {
            // Otherwise, do not allow them to walk on it
            canWalkOnSlope = false;
        }
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
		if(isGrounded && RB.velocity.y > 0)
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
        if (!isLocked)
        {
			// Set the move input to the value returned by context
			moveInput = context.ReadValue<Vector2>();
			
			// Check direction to face based on vector
			if (moveInput.x != 0)
			{
				// Check directions to face
                CheckDirectionToFace(moveInput.x > 0);
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
        if (!isLocked)
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

    // EVENT FUNCTIONS
	#region EVENT FUNCTIONS
	/// <summary>
	/// Set variables for bouncing
	/// </summary>
	private void ApplyBounce()
	{
		bouncing = true;
		bounceBuffer = 0.1f;
		landedTimer = 0.2f;
	}

	/// <summary>
	/// Set whether the player is touching a shroom
	/// </summary>
	/// <param name="touchingData"></param>

	private void TouchingShroom(bool touchingData)
	{
		touchingShroom = touchingData;
	}

	/// <summary>
	/// Lock player movement
	/// </summary>
	/// <param name="lockedData"></param>
	private void LockMovement()
	{
        moveInput = Vector2.zero;
        isLocked = true;
	}

    /// <summary>
    /// Unlock player movement
    /// </summary>
    private void UnlockMovement()
    {
        isLocked = false;
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
	#endregion

	// DATA HANDLING
	#region DATA HANDLING
	public void LoadData(GameData data)
	{
		// Load player position
		gameObject.transform.position = data.playerPosition;
		isLocked = data.isLocked;
	}

	public void SaveData(GameData data)
	{
		// Save player position
		data.playerPosition = gameObject.transform.position;
		data.isLocked = isLocked;
	}
	#endregion
}
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region FIELDS
    public PlayerData Data;
	public bool IsFacingRight { get; private set; }
	public bool IsJumping { get; private set; }
	public bool _isMoving;

	// Timers
	public float LastOnGroundTime { get; private set; }
	//public float LastOnWallTime { get; private set; }
	//public float LastOnWallRightTime { get; private set; }
	//public float LastOnWallLeftTime { get; private set; }

	// Jump
	private bool _isJumpCut;
	public bool _isJumpFalling;

	private Vector2 _moveInput;
	public float LastPressedJumpTime { get; private set; }

	[SerializeField] float landedTimer = 0f;
	[SerializeField] bool justLanded = false;
    #endregion

    #region VARIABLES
    // Components
    public Rigidbody2D RB { get; private set; }
	public Animator animator;
	public BouncingEffect bounceEffect;
	private PauseMenu pauseMenu;

	// Set all of these up in the inspector
	[Header("Checks")]
	public Transform _groundCheckPoint;

	// Size of groundCheck depends on the size of your character generally you want them slightly small than width (for ground) and height (for the wall check)
	public Vector2 _groundCheckSize = new Vector2(0.49f, 0.03f);

	[Header("Layers & Tags")]
	[SerializeField] private LayerMask _groundLayer;
	#endregion

	public Vector2 playerPosition;
	public Vector2 previousPlayerPosition;
	public Vector2 distanceFromLastPosition;

	public bool isGrounded;
	public bool inputHorizontal;

	public bool isLocked = false;

	private void Awake()
	{
		RB = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
		bounceEffect = GetComponent<BouncingEffect>();
		pauseMenu = GameObject.Find("PauseUI").GetComponent<PauseMenu>();
	}

	private void Start()
	{
		SetGravityScale(Data.gravityScale);
		IsFacingRight = true;
		playerPosition = transform.position;
	}

	private void Update()
	{
		RB.constraints = RigidbodyConstraints2D.None;
		RB.constraints = RigidbodyConstraints2D.FreezeRotation;
		playerPosition = transform.position;
		distanceFromLastPosition = playerPosition - previousPlayerPosition;

		_isMoving = false;
		if (RB.velocity != Vector2.zero)
		{
			_isMoving = true;
		}

		#region TIMERS
		LastOnGroundTime -= Time.deltaTime;

		LastPressedJumpTime -= Time.deltaTime;
		#endregion

		#region COLLISION CHECKS
		if (!IsJumping)
		{
			// Ground Check
			if (Physics2D.OverlapBox(_groundCheckPoint.position, _groundCheckSize, 0, _groundLayer) && !IsJumping) // Checks if set box overlaps with ground
			{
				// If bouncing beore, end bouncing
				if (bounceEffect.bouncing)
				{
					bounceEffect.bouncing = false;
				}

				// Ground player
				isGrounded = true;

				// Set coyote time
				LastOnGroundTime = Data.coyoteTime;

				if (landedTimer > 0)
				{
					landedTimer -= Time.deltaTime;
					justLanded = true;
					animator.SetBool("JustLanded", true);
				}
				else
				{
					justLanded = false;
					animator.SetBool("JustLanded", false);
				}
			}
		}
		#endregion

		#region JUMP CHECKS
		if (IsJumping && RB.velocity.y < 0)
		{
			IsJumping = false;

			_isJumpFalling = true;
		}

		if (LastOnGroundTime > 0 && !IsJumping)
		{
			_isJumpCut = false;

			if (!IsJumping)
				_isJumpFalling = false;
		}

		if (RB.velocity.y < 0)
		{
			_isJumpFalling = true;
		}

		#region JUMP ANIMATION CHECKS
		if (IsJumping)
		{
			isGrounded = false;
			animator.SetBool("Jump", true);
		}
		else if (!IsJumping)
		{
			animator.SetBool("Jump", false);
		}

		if (_isJumpFalling)
		{
			isGrounded = false;
			animator.SetBool("Falling", true);
		}
		else if (!_isJumpFalling)
		{
			animator.SetBool("Falling", false);
		}
		#endregion

		// Jump
		if (CanJump() && LastPressedJumpTime > 0)
		{
			IsJumping = true;
			_isJumpCut = false;
			_isJumpFalling = false;
			Jump();
		}
		#endregion

		#region GRAVITY

		if (!bounceEffect.bouncing)
		{
			// Higher gravity if we've released the jump input or are falling
			if (RB.velocity.y < 0 && _moveInput.y < 0)
			{
				// Much higher gravity if holding down
				SetGravityScale(Data.gravityScale * Data.fastFallGravityMult);

				// Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
				RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFastFallSpeed));
			}
			else if (_isJumpCut)
			{
				// Higher gravity if jump button released
				SetGravityScale(Data.gravityScale * Data.jumpCutGravityMult);
				RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
			}
			else if ((IsJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
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
			if (RB.velocity.y > 0)
			{
				// Higher gravity if falling
				SetGravityScale(Data.gravityScale * Data.bounceGravityMult);

				// Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
				RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
			}
			else if (RB.velocity.y < 0)
			{
				// Higher gravity if falling
				SetGravityScale(Data.gravityScale * Data.fallFromBounceGravityMult);

				// Caps maximum fall speed, so when falling over large distances we don't accelerate to insanely high speeds
				RB.velocity = new Vector2(RB.velocity.x, Mathf.Max(RB.velocity.y, -Data.maxFallSpeed));
			}
		}
		#endregion

		previousPlayerPosition = playerPosition;
	}

    private void FixedUpdate()
	{
		// Handle Run
		Run(1);
	}

	#region INPUT CALLBACKS
	// Methods which whandle input detected in Update()
	public void OnJumpInput()
	{
		LastPressedJumpTime = Data.jumpInputBufferTime;
	}

	public void OnJumpUpInput()
	{
		if (CanJumpCut())
        {
			_isJumpCut = true;
		}
	}
	#endregion

	#region GENERAL METHODS
	public void SetGravityScale(float scale)
	{
		RB.gravityScale = scale;
	}
	#endregion

	// MOVEMENT METHODS
	#region RUN METHODS
	private void Run(float lerpAmount)
	{
		// Calculate the direction we want to move in and our desired velocity
		float targetSpeed = _moveInput.x * Data.runMaxSpeed;
		// Reduce are control using Lerp() this smooths changes to are direction and speed
		targetSpeed = Mathf.Lerp(RB.velocity.x, targetSpeed, lerpAmount);

		#region Calculate AccelRate
		float accelRate;

		// Gets an acceleration value based on if we are accelerating (includes turning) 
		// or trying to decelerate (stop). As well as applying a multiplier if we're air borne.
		if (LastOnGroundTime > 0)
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
		if ((IsJumping || _isJumpFalling) && Mathf.Abs(RB.velocity.y) < Data.jumpHangTimeThreshold)
		{
			accelRate *= Data.jumpHangAccelerationMult;
			targetSpeed *= Data.jumpHangMaxSpeedMult;
		}
		#endregion

		#region Conserve Momentum
		// We won't slow the player down if they are moving in their desired direction but at a greater speed than their maxSpeed
		if (Data.doConserveMomentum && Mathf.Abs(RB.velocity.x) > Mathf.Abs(targetSpeed) && Mathf.Sign(RB.velocity.x) == Mathf.Sign(targetSpeed) && Mathf.Abs(targetSpeed) > 0.01f && LastOnGroundTime < 0)
		{
			// Prevent any deceleration from happening, or in other words conserve are current momentum
			if(!bounceEffect.bouncing)
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

	public void Turn()
	{
		// Stores scale and flips the player along the x axis, 
		Vector3 scale = transform.localScale;
		scale.x *= -1;
		transform.localScale = scale;

		IsFacingRight = !IsFacingRight;
	}
	#endregion

	#region JUMP METHODS
	private void Jump()
	{
		// Ensures we can't call Jump multiple times from one press
		LastPressedJumpTime = 0;
		LastOnGroundTime = 0;
		landedTimer = 0.3f;

		#region Perform Jump
		// We increase the force applied if we are falling
		// This means we'll always feel like we jump the same amount 
		float force = Data.jumpForce;
		if (RB.velocity.y < 0)
        {
			force -= RB.velocity.y;
		}

		RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
		#endregion
	}
	#endregion

	#region CHECK METHODS
	public void CheckDirectionToFace(bool isMovingRight)
	{
		if (isMovingRight != IsFacingRight)
			Turn();
	}

	private bool CanJump()
	{
		return LastOnGroundTime > 0 && !IsJumping;
	}

	private bool CanJumpCut()
	{
		return IsJumping && RB.velocity.y > 0;
	}
	#endregion

	// INPUT HANDLER
	#region INPUT HANDLER
	public void OnMove(InputAction.CallbackContext context)
    {
        if (!pauseMenu.paused)
        {
			// Set the move input to the value returned by context
			_moveInput = context.ReadValue<Vector2>();

			// Set animation and inputHorizontal variables
			animator.SetFloat("Speed", Mathf.Abs(_moveInput.x));
			inputHorizontal = true;

			// Check direction to face based on vector
			if (_moveInput.x != 0)
			{
				CheckDirectionToFace(_moveInput.x > 0);
			}

			// If the bind is no longer pressed, set inputHorizontal to false
			if (context.canceled)
			{
				inputHorizontal = false;
			}
		}
	}

	public void OnJump(InputAction.CallbackContext context)
	{
        if (!pauseMenu.paused)
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


	#region EDITOR METHODS
	private void OnDrawGizmosSelected()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(_groundCheckPoint.position, _groundCheckSize);
		Gizmos.color = Color.blue;
	}
	#endregion
}
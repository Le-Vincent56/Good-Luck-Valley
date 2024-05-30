using GHoodLuckValley.Player.Data;
using GoodLuckValley.Mushroom;
using GoodLuckValley.Player.Input;
using GoodLuckValley.Patterns.StateMachine;
using UnityEngine;
using GoodLuckValley.Player.States;
using GoodLuckValley.Entity;
using GoodLuckValley.Events;

namespace GoodLuckValley.Player.Control
{
    public class PlayerController : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private GameEvent onWallJumpInput;

        [Header("References")]
        [SerializeField] private Animator animator;
        [SerializeField] private InputReader input;
        [SerializeField] private CollisionHandler collisionHandler;
        [SerializeField] private PlayerData data;

        [Header("Fields - Physics")]
        [SerializeField] private float gravity;
        [SerializeField] private float maxJumpVelocity;
        [SerializeField] private float minJumpVelocity;
        [SerializeField] private bool fastFalling;
        [SerializeField] private Vector2 velocity;

        [Header("Fields - Movement")]
        [SerializeField] private float xVelSmoothing;

        [Header("Fields - Jump")]
        [SerializeField] private bool isJumpCut;
        [SerializeField] private bool isJumping;
        [SerializeField] private float lastOnGroundTime;
        [SerializeField] private float lastPressedJumpTime;

        [Header("Fields - Wall Slide")]
        [SerializeField] private bool wallSliding;
        [SerializeField] private int wallDirX;
        [SerializeField] private Vector2 wallJumpClimb;
        [SerializeField] private Vector2 wallJumpOff;
        [SerializeField] private Vector2 wallJumpLeap;
        [SerializeField] private float timeToWallUnstick;

        [Header("Fields - Checks")]
        [SerializeField] private bool isGrounded;
        [SerializeField] private bool isBouncing;

        private StateMachine stateMachine;

        private void Awake()
        {
            // Get components
            animator = GetComponent<Animator>();
            collisionHandler = GetComponent<CollisionHandler>();

            // Declare states
            stateMachine = new StateMachine();
            IdleState idleState = new IdleState(this, animator);
            LocomotionState locomotionState = new LocomotionState(this, animator);
            JumpState jumpState = new JumpState(this, animator);
            WallState wallState = new WallState(this, animator);
            FallState fallState = new FallState(this, animator);
            BounceState bounceState = new BounceState(this, animator);

            // Define strict transitions
            At(idleState, locomotionState, new FuncPredicate(() => input.NormInputX != 0));
            At(idleState, jumpState, new FuncPredicate(() => isJumping));
            At(locomotionState, idleState, new FuncPredicate(() => input.NormInputX == 0));
            At(locomotionState, jumpState, new FuncPredicate(() => isJumping));
            At(jumpState, locomotionState, new FuncPredicate(() => isGrounded && !isJumping));
            At(jumpState, wallState, new FuncPredicate(() => wallSliding));
            At(wallState, idleState, new FuncPredicate(() => isGrounded && input.NormInputX  == 0));
            At(wallState, jumpState, new FuncPredicate(() => isJumping));
            At(fallState, idleState, new FuncPredicate(() => isGrounded && input.NormInputX  == 0));
            At(fallState, locomotionState, new FuncPredicate(() => isGrounded && input.NormInputX  != 0));
            At(fallState, wallState, new FuncPredicate(() => wallSliding));
            At(bounceState, idleState, new FuncPredicate(() => isGrounded && input.NormInputX == 0));
            At(bounceState, locomotionState, new FuncPredicate(() => isGrounded && input.NormInputX != 0));
            At(bounceState, wallState, new FuncPredicate(() => wallSliding));

            // Define any transitions
            Any(fallState, new FuncPredicate(() => !isGrounded && !wallSliding && velocity.y < 0f));
            Any(bounceState, new FuncPredicate(() => isBouncing));

            // Set an initial state
            stateMachine.SetState(idleState);
        }

        private void Start()
        {
            // Define gravity
            gravity = -(2 * data.maxJumpHeight) / Mathf.Pow(data.timeToJumpApex, 2);
            maxJumpVelocity = Mathf.Abs(gravity * data.timeToJumpApex);
            minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * data.minJumpHeight);
        }

        private void OnEnable()
        {
            input.Jump += OnJump;
            input.FastFall += OnFastFall;
        }

        private void OnDisable()
        {
            input.Jump -= OnJump;
            input.FastFall -= OnFastFall;
        }

        private void Update()
        {
            // Update timers
            UpdateTimers();

            // Update the state machine
            stateMachine.Update();
        }

        private void FixedUpdate()
        {
            // Update the state machine
            stateMachine.FixedUpdate();

            // Update sprite
            CheckDirectionToFace();
        }

        /// <summary>
        /// Add a transition from one State to another given a certain condition
        /// </summary>
        /// <param name="from">The State to define the transition from</param>
        /// <param name="to">The State to define the transition to</param>
        /// <param name="condition">The condition of the Transition</param>
        private void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);

        /// <summary>
        /// Add a transition from any State to another one given a certain condition
        /// </summary>
        /// <param name="to">The State to define the transition to</param>
        /// <param name="condition">The condition of the transition</param>
        private void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        /// <summary>
        /// Check the direction for the player to face
        /// </summary>
        private void CheckDirectionToFace()
        {
            Vector3 scale = transform.localScale;
            scale.x = collisionHandler.collisions.FacingDirection;
            transform.localScale = scale;
        }

        /// <summary>
        /// Update movement timers
        /// </summary>
        public void UpdateTimers()
        {
            if(lastOnGroundTime > 0f)
                lastOnGroundTime -= Time.deltaTime;

            if(lastPressedJumpTime > 0f)
                lastPressedJumpTime -= Time.deltaTime;
        }

        /// <summary>
        /// Calculate the initial velocity of movement before handling
        /// </summary>
        public void CalculateVelocity()
        {
            // Get the target speed
            float targetSpeed = input.NormInputX  * data.movementSpeed;

            // Smooth the target speed, taking in acceleration to account
            velocity.x = Mathf.SmoothDamp(
                velocity.x,
                targetSpeed,
                ref xVelSmoothing,
                (collisionHandler.collisions.Below) ? data.accelerationTimeGround : data.accelerationTimeAir
            );

            // Check if the player is grounded
            if (collisionHandler.collisions.Below)
            {
                // Set grounded to true
                isGrounded = true;

                // Set bouncing to false
                isBouncing = false;

                // Set coyote time
                lastOnGroundTime = data.coyoteTime;

                if(lastPressedJumpTime > 0f)
                {
                    // Set jumping to true
                    isJumping = true;

                    // Reset last pressed jump time
                    lastPressedJumpTime = 0f;

                    // Execute the jump
                    HandleJump();
                }
            }
            else isGrounded = false;

            if(collisionHandler.collisions.Below && velocity.y <= 0f)
            {
                if (isJumping)
                    isJumping = false;

                if (isJumpCut)
                    isJumpCut = false;
            }

            // Check if wall sliding
            if ((collisionHandler.collisions.Left || collisionHandler.collisions.Right) &&
                !collisionHandler.collisions.Below && velocity.y < 0 &&
                collisionHandler.collisions.Layer == CollisionHandler.CollisionLayer.MushroomWall)
            {
                if (isJumping)
                    isJumping = false;

                if (isJumpCut)
                    isJumpCut = false;

                wallSliding = true;
            } else
            {
                wallSliding = false;
            }

            // Apply gravity if not wall sliding - wall sliding handles gravity
            // on it's own
            if(!wallSliding)
                HandleFalling();
        }

        /// <summary>
        /// Handle player falling
        /// </summary>
        public void HandleFalling()
        {
            // Check if grounded
            if(isGrounded)
            {
                velocity.y += gravity * Time.deltaTime;
                return;
            }

            // Check if fast falling
            if(fastFalling)
            {
                // Increase the gravity by the scalar
                velocity.y += gravity * data.fastFallScalar * Time.deltaTime;

                // Clamp to fast fall speed
                if(velocity.y < -data.maxFastFallSpeed)
                {
                    velocity.y = -data.maxFastFallSpeed;
                }
            } else
            {
                // Use normal gravity
                velocity.y += gravity * Time.deltaTime;

                // If not, clamp to normal fall speed
                if (velocity.y < -data.maxFallSpeed)
                {
                    velocity.y = -data.maxFallSpeed;
                }
            }
        }

        /// <summary>
        /// Handle the wall sliding of the player
        /// </summary>
        public void HandleWallSliding()
        {
            // Get the direction of the wall
            wallDirX = (collisionHandler.collisions.Left) ? -1 : 1;

            // Check if wall sliding
            if (wallSliding)
            {
                // Check if fast falling
                if(fastFalling)
                {
                    // Increase the gravity by the scalar
                    velocity.y += gravity * data.fastWallSlideScalar * Time.deltaTime;

                    // Clamp to the max fast wall slide speed
                    if (velocity.y < -data.maxFastWallSlideSpeed)
                    {
                        velocity.y = -data.maxFastWallSlideSpeed;
                    }
                } else
                {
                    // Use normal gravity
                    velocity.y += gravity * Time.deltaTime;

                    // Clamp to the max wall slide speed
                    if (velocity.y < -data.maxWallSlideSpeed)
                    {
                        velocity.y = -data.maxWallSlideSpeed;
                    }
                }

                // Check if we should be sticking to the wall
                if (timeToWallUnstick > 0f)
                {
                    // Set x-velocity and smoothing to 0
                    xVelSmoothing = 0f;
                    velocity.x = 0f;

                    // Check if we need to stick to the wall
                    if (input.NormInputX != wallDirX && input.NormInputX != 0f)
                    {
                        timeToWallUnstick -= Time.deltaTime;
                    }
                    else
                    {
                        // Reset the wall stick time
                        timeToWallUnstick = data.wallStickTime;
                    }
                }
            }
        }

        /// <summary>
        /// Handle the player movement
        /// </summary>
        public void HandleMovement()
        {
            // Move
            Move(velocity * Time.deltaTime);

            // If there's a collision above or below, set the y-velocity to 0 to stop
            // from accumulating gravity
            if (collisionHandler.collisions.Above || collisionHandler.collisions.Below && !isJumping)
            {
                // Check if sliding down slope
                if (collisionHandler.collisions.SlidingDownMaxSlope)
                {
                    // If so, set the y-velocity to the slope normal multiplied by gravity
                    velocity.y += collisionHandler.collisions.SlopeNormal.y * -gravity * Time.deltaTime;
                }
                else
                {
                    // Otherwise, set y-velocity to 0
                    velocity.y = 0f;
                }
            }
        }

        /// <summary>
        /// Move the player
        /// </summary>
        /// <param name="velocity"></param>
        /// <param name="standingOnPlatform"></param>
        public void Move(Vector2 velocity, bool standingOnPlatform = false)
        {
            // Update raycasts
            collisionHandler.UpdateRaycastOrigins();

            // Reset collisions
            collisionHandler.collisions.ResetInfo();

            // Set the old velocity
            collisionHandler.collisions.PrevVelocity = velocity;

            // Handle collisions if necessary
            if(velocity.y < 0f)
                collisionHandler.DescendSlope(ref velocity);

            // Set the facing direction
            if (velocity.x != 0f)
            {
                collisionHandler.collisions.FacingDirection = (int)Mathf.Sign(velocity.x);
            }

            // Handle horizontal collisions
            collisionHandler.HorizontalCollisions(ref velocity);

            // Handle vertical collisions
            if (velocity.y != 0f)
                collisionHandler.VerticalCollisions(ref velocity);

            // Move
            transform.Translate(velocity);

            if(standingOnPlatform)
            {
                collisionHandler.collisions.Below = true;
            }
        }


        /// <summary>
        /// Handle the upward movement of the jump
        /// </summary>
        private void HandleJump()
        {
            // Check if grounded
            if (collisionHandler.collisions.Below || lastOnGroundTime > 0 && !isJumpCut)
            {
                // Check if sliding down a slope
                if (collisionHandler.collisions.SlidingDownMaxSlope)
                {
                    // Check if we are not jumping against a max slope
                    if (input.NormInputX != -Mathf.Sign(collisionHandler.collisions.SlopeNormal.x))
                    {
                        velocity.y = maxJumpVelocity * collisionHandler.collisions.SlopeNormal.y;
                        velocity.x = maxJumpVelocity * collisionHandler.collisions.SlopeNormal.x;
                    }
                }
                else
                {
                    // Otherwise, jump like normal
                    velocity.y = maxJumpVelocity;
                }
            }

            if(isJumpCut)
            {
                // Cut velocity
                if (velocity.y > minJumpVelocity)
                {
                    velocity.y = minJumpVelocity;
                }
            }
        }

        /// <summary>
        /// Begin the jump
        /// </summary>
        public void StartJump()
        {
            // Check if wall sliding
            if(!wallSliding)
            {
                HandleJump();
            }
            else
            {
            }

            // Check if wall sliding
            if (wallSliding)
            {
                velocity.x = -wallDirX * wallJumpLeap.x;
                velocity.y = wallJumpLeap.y;
            }

            
        }

        /// <summary>
        /// Handle jump input
        /// </summary>
        /// <param name="started"></param>
        public void OnJump(bool started)
        {
            // Check if the context was started
            if(started)
            {
                if (!isJumping)
                {
                    // Set jumping to true
                    isJumping = true;

                    // Set jump cut to false
                    isJumpCut = false;

                    // Start the jump
                    StartJump();
                }
                else if (isJumping)
                {
                    isJumpCut = false;

                    // Set the last pressed jump time
                    lastPressedJumpTime = data.jumpBufferTime;
                }
            }

            // Check if the context was canceled and that the input still applies
            if(!started)
            {
                // Set jump cutting
                isJumpCut = true;

                // Handle the jump
                HandleJump();
            }
        }

        /// <summary>
        /// Handle fast fall input
        /// </summary>
        /// <param name="started"></param>
        public void OnFastFall(bool started)
        {
            if (started)
            {
                fastFalling = true;
            }

            if (!started)
            {
                fastFalling = false;
            }
        }

        /// <summary>
        /// Set the player to bouncing
        /// </summary>
        /// <param name="isBouncing"></param>
        public void SetBouncing(bool isBouncing) => this.isBouncing = isBouncing; 

        /// <summary>
        /// Bounce the player
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void Bounce(Component sender, object data)
        {
            // Check if the data is the correct type
            if (data is not MushroomBounce.BounceData) return;

            // Cast data
            MushroomBounce.BounceData bounceData =(MushroomBounce.BounceData)data;

            // Set bouncing to true
            isBouncing = true;

            // Apply bounce force
            velocity.x = bounceData.BounceVector.x;
            velocity.y = bounceData.BounceVector.y;
        }
    }
}
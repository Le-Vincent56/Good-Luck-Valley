using GoodLuckValley.Player.Input;
using GoodLuckValley.Player.StateMachine;
using GoodLuckValley.Player.StateMachine.States;
using UnityEngine;
using UnityEngine.Rendering;

namespace GoodLuckValley.Player.Control
{
    public class PlayerController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private PlayerData data;
        [SerializeField] private Animator animator;
        [SerializeField] private InputReader input;
        [SerializeField] private CollisionHandler collisionHandler;

        [Header("Fields - Physics")]
        [SerializeField] private float gravity;
        [SerializeField] private float accelerationTimeGround;
        [SerializeField] private float accelerationTimeAir;
        [SerializeField] private float maxJumpVelocity;
        [SerializeField] private float minJumpVelocity;
        [SerializeField] private bool fastFalling;
        [SerializeField] private float maxFallSpeed;
        [SerializeField] private float fastFallScalar;
        [SerializeField] private float maxFastFallSpeed;
        [SerializeField] private Vector2 velocity;

        [Header("Fields - Movement")]
        [SerializeField] private float movementSpeed;
        [SerializeField] private float xVelSmoothing;

        [Header("Fields - Jump")]
        [SerializeField] private bool isJumping;
        [SerializeField] private float maxJumpHeight;
        [SerializeField] private float minJumpHeight;
        [SerializeField] private float timeToJumpApex;

        [Header("Fields - Wall Slide")]
        [SerializeField] private bool wallSliding;
        [SerializeField] private int wallDirX;
        [SerializeField] private Vector2 wallJumpClimb;
        [SerializeField] private Vector2 wallJumpOff;
        [SerializeField] private Vector2 wallJumpLeap;
        [SerializeField] private float fastWallSlideScalar;
        [SerializeField] private float maxWallSlideSpeed;
        [SerializeField] private float maxFastWallSlideSpeed;
        [SerializeField] private float wallStickTime;
        [SerializeField] private float timeToWallUnstick;

        [Header("Fields - Checks")]
        [SerializeField] private bool isGrounded;

        private StateMachine.StateMachine stateMachine;

        private void Awake()
        {
            // Get components
            animator = GetComponent<Animator>();
            collisionHandler = GetComponent<CollisionHandler>();

            // Declare states
            stateMachine = new StateMachine.StateMachine();
            IdleState idleState = new IdleState(this, animator);
            LocomotionState locomotionState = new LocomotionState(this, animator);
            JumpState jumpState = new JumpState(this, animator);
            WallState wallState = new WallState(this, animator);
            FallState fallState = new FallState(this, animator);

            // Define transitions
            At(idleState, locomotionState, new FuncPredicate(() => input.NormInputX  != 0));
            At(idleState, jumpState, new FuncPredicate(() => isJumping));
            At(locomotionState, idleState, new FuncPredicate(() => input.NormInputX  == 0));
            At(locomotionState, jumpState, new FuncPredicate(() => isJumping));
            At(jumpState, locomotionState, new FuncPredicate(() => isGrounded && !isJumping));
            At(jumpState, wallState, new FuncPredicate(() => wallSliding));
            At(wallState, idleState, new FuncPredicate(() => isGrounded && input.NormInputX  == 0));
            At(wallState, jumpState, new FuncPredicate(() => isJumping));
            At(fallState, idleState, new FuncPredicate(() => isGrounded && input.NormInputX  == 0));
            At(fallState, locomotionState, new FuncPredicate(() => isGrounded && input.NormInputX  != 0));
            At(fallState, wallState, new FuncPredicate(() => wallSliding));

            // Any, go to fall
            Any(fallState, new FuncPredicate(() => !isGrounded && !wallSliding && velocity.y < 0f));

            stateMachine.SetState(idleState);
        }

        private void Start()
        {
            // Define gravity
            gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            maxJumpVelocity = Mathf.Abs(gravity * timeToJumpApex);
            minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
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

        private void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);
        private void Any(IState to, IPredicate condition) => stateMachine.AddAnyTransition(to, condition);

        private void CheckDirectionToFace()
        {
            Vector3 scale = transform.localScale;
            scale.x = collisionHandler.collisions.FacingDirection;
            transform.localScale = scale;
        }

        public void CalculateVelocity()
        {
            // Get the target speed
            float targetSpeed = input.NormInputX  * movementSpeed;

            // Smooth the target speed, taking in acceleration to account
            velocity.x = Mathf.SmoothDamp(
                velocity.x,
                targetSpeed,
                ref xVelSmoothing,
                (collisionHandler.collisions.Below) ? accelerationTimeGround : accelerationTimeAir
            );

            // Check if the player is grounded
            if (collisionHandler.collisions.Below)
            {
                // Set grounded to true
                isGrounded = true;

                // If the player was jumping, stop jumping
                if (isJumping) isJumping = false;
            }
            else isGrounded = false;

            // Check if wall sliding
            if ((collisionHandler.collisions.Left || collisionHandler.collisions.Right) &&
                !collisionHandler.collisions.Below && velocity.y < 0 &&
                collisionHandler.collisions.Layer == CollisionHandler.CollisionLayer.MushroomWall)
            {
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
                velocity.y += gravity * fastFallScalar * Time.deltaTime;

                // Clamp to fast fall speed
                if(velocity.y < -maxFastFallSpeed)
                {
                    velocity.y = -maxFastFallSpeed;
                }
            } else
            {
                // Use normal gravity
                velocity.y += gravity * Time.deltaTime;

                // If not, clamp to normal fall speed
                if (velocity.y < -maxFallSpeed)
                {
                    velocity.y = -maxFallSpeed;
                }
            }
        }

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
                    velocity.y += gravity * fastWallSlideScalar * Time.deltaTime;

                    // Clamp to the max fast wall slide speed
                    if (velocity.y < -maxFastWallSlideSpeed)
                    {
                        velocity.y = -maxFastWallSlideSpeed;
                    }
                } else
                {
                    // Use normal gravity
                    velocity.y += gravity * Time.deltaTime;

                    // Clamp to the max wall slide speed
                    if (velocity.y < -maxWallSlideSpeed)
                    {
                        velocity.y = -maxWallSlideSpeed;
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
                        timeToWallUnstick = wallStickTime;
                    }
                }
            }
        }

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
            else if (!collisionHandler.collisions.Below)
            {
                // Set not grounded
                isGrounded = false;
            }
        }

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

        public void OnJump(bool started)
        {
            // Check if the context was started
            if(started)
            {
                // Set jumping to true
                isJumping = true;

                // Check if wall sliding
                if (wallSliding)
                {
                    // Check if pressing against the wall
                    if (wallDirX == input.NormInputX )
                    {
                        velocity.x = -wallDirX * wallJumpClimb.x;
                        velocity.y = wallJumpClimb.y;
                    }
                    else if (input.NormInputX  == 0) // Check if not pressing at all
                    {
                        velocity.x = -wallDirX * wallJumpOff.x;
                        velocity.y = wallJumpOff.y;
                    }
                    else // Check if pressing away from the wall
                    {
                        velocity.x = -wallDirX * wallJumpLeap.x;
                        velocity.y = wallJumpLeap.y;
                    }
                }

                // Check if grounded
                if (collisionHandler.collisions.Below)
                {
                    // Check if sliding down a slope
                    if (collisionHandler.collisions.SlidingDownMaxSlope)
                    {
                        // Check if we are not jumping against a max slope
                        if (input.NormInputX  != -Mathf.Sign(collisionHandler.collisions.SlopeNormal.x))
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
            }

            // Check if the context was canceled
            if(!started)
            {
                // Cut velocity
                if (velocity.y > minJumpVelocity)
                {
                    velocity.y = minJumpVelocity;
                }

                // Set jumping to false
                isJumping = false;
            }
        }

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
    }
}
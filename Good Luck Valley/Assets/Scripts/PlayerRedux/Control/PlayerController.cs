using GoodLuckValley.Player.Input;
using GoodLuckValley.Player.StateMachine;
using GoodLuckValley.Player.StateMachine.States;
using UnityEngine;

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
        [SerializeField] private float wallSlideSpeedMax;
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


            // Define transitions
            At(idleState, locomotionState, new FuncPredicate(() => input.Direction.x != 0));
            At(idleState, jumpState, new FuncPredicate(() => isJumping));
            At(locomotionState, idleState, new FuncPredicate(() => input.Direction.x == 0));
            At(locomotionState, jumpState, new FuncPredicate(() => isJumping));
            At(jumpState, locomotionState, new FuncPredicate(() => isGrounded && !isJumping));
            At(jumpState, wallState, new FuncPredicate(() => wallSliding));
            At(wallState, locomotionState, new FuncPredicate(() => !wallSliding && isGrounded));
            At(wallState, jumpState, new FuncPredicate(() => isJumping));

            


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

        public void HandleMovement()
        {
            // Move
            Move(velocity * Time.deltaTime);

            // If there's a collision above or below, set the y-velocity to 0 to stop
            // from accumulating gravity
            if (collisionHandler.collisions.Above || collisionHandler.collisions.Below && !isJumping)
            {
                // Set grounded
                isGrounded = true;

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
            } else if(!collisionHandler.collisions.Below)
            {
                // Set not grounded
                isGrounded = false;
            }
        }

        public void CalculateVelocity()
        {
            // Get the target speed
            float targetSpeed = input.Direction.x * movementSpeed;

            // Smooth the target speed, taking in acceleration to account
            velocity.x = Mathf.SmoothDamp(
                velocity.x,
                targetSpeed,
                ref xVelSmoothing,
                (collisionHandler.collisions.Below) ? accelerationTimeGround : accelerationTimeAir
            );

            // Apply gravity
            velocity.y += gravity * Time.deltaTime;
        }

        public void HandleWallSliding()
        {
            // Get the direction of the wall
            wallDirX = (collisionHandler.collisions.Left) ? -1 : 1;

            // Check if wall sliding
            wallSliding = false;
            if ((collisionHandler.collisions.Left || collisionHandler.collisions.Right) &&
                !collisionHandler.collisions.Below)
            {
                wallSliding = true;
                if (velocity.y < 0f && velocity.y < -wallSlideSpeedMax)
                {
                    velocity.y = -wallSlideSpeedMax;
                }

                // Check if we should be sticking to the wall
                if (timeToWallUnstick > 0f)
                {
                    // Set x-velocity and smoothing to 0
                    xVelSmoothing = 0f;
                    velocity.x = 0f;

                    // Check if we need to stick to the wall
                    if (input.Direction.x != wallDirX && input.Direction.x != 0f)
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
            if(started)
            {
                isJumping = true;

                // Check if wall sliding
                if (wallSliding)
                {
                    // Check if pressing against the wall
                    if (wallDirX == input.Direction.x)
                    {
                        velocity.x = -wallDirX * wallJumpClimb.x;
                        velocity.y = wallJumpClimb.y;
                    }
                    else if (input.Direction.x == 0) // Check if not pressing at all
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
                        if (input.Direction.x != -Mathf.Sign(collisionHandler.collisions.SlopeNormal.x))
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

            if(!started)
            {
                if (velocity.y > minJumpVelocity)
                {
                    velocity.y = minJumpVelocity;
                }

                isJumping = false;
            }
        }
    }
}
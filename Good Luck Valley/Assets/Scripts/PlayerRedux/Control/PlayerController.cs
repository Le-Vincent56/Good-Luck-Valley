using GoodLuckValley.Player.Input;
using GoodLuckValley.Player.StateMachine;
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
        [SerializeField] private float jumpVelocity;
        [SerializeField] private Vector2 velocity;

        [Header("Fields - Movement")]
        [SerializeField] private float movementSpeed;
        [SerializeField] private float xVelSmoothing;

        [Header("Fields - Jump")]
        [SerializeField] private bool isJumping;
        [SerializeField] private float jumpHeight;
        [SerializeField] private float timeToJumpApex;

        private StateMachine.StateMachine stateMachine;

        private void Awake()
        {
            // Get components
            animator = GetComponent<Animator>();
            collisionHandler = GetComponent<CollisionHandler>();

            // Declare states
            stateMachine = new StateMachine.StateMachine();
            LocomotionState locomotionState = new LocomotionState(this, animator);
            JumpState jumpState = new JumpState(this, animator);

            // Define transitions
        }

        private void Start()
        {
            // Define gravity
            gravity = -(2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2);
            jumpVelocity = Mathf.Abs(gravity * timeToJumpApex);
        }

        private void OnEnable()
        {
            input.Jump += OnJump;
        }

        private void FixedUpdate()
        {
            // If there's a collision above or below, set the y-velocity to 0 to stop
            // from accumulating gravity
            if (collisionHandler.collisions.Above || collisionHandler.collisions.Below && !isJumping)
                velocity.y = 0;

            // Apply gravity
            velocity.y += gravity * Time.deltaTime;

            // Move
            Move(velocity * Time.deltaTime);
        }

        public void Move(Vector2 velocity)
        {
            // Update raycasts
            collisionHandler.UpdateRaycastOrigins();

            // Reset collisions
            collisionHandler.collisions.ResetInfo();

            // Set the old velocity
            collisionHandler.collisions.PrevVelocity = velocity;

            // Get the target speed
            float targetSpeed = input.Direction.x * movementSpeed;

            // Smooth the target speed, taking in acceleration to account
            velocity.x = Mathf.SmoothDamp(
                velocity.x,
                targetSpeed,
                ref xVelSmoothing,
                (collisionHandler.collisions.Below) ? accelerationTimeGround : accelerationTimeAir
           );

            // Handle collisions if necessary
            if(velocity.y < 0)
            {
                collisionHandler.DescendSlope(ref velocity);
            }
            if (velocity.x != 0)
                collisionHandler.HorizontalCollisions(ref velocity);
            if (velocity.y != 0)
                collisionHandler.VerticalCollisions(ref velocity);

            // Move
            transform.Translate(velocity);
        }

        public void OnJump(bool started)
        {
            if(started)
            {
                isJumping = true;
                velocity.y = jumpVelocity;
            }
            else
            {
                isJumping = false;
            }
        }
    }
}
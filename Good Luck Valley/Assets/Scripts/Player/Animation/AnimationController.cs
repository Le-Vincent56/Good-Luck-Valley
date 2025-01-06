using GoodLuckValley.Architecture.EventBus;
using GoodLuckValley.Player.Movement;
using GoodLuckValley.Timers;
using UnityEngine;

namespace GoodLuckValley.Player.Animation
{
    public class AnimationController : MonoBehaviour
    {
        private PlayerController playerController;
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        [SerializeField] private float crossFadeDuration;
        private bool updateFacingDirection;
        private float lastFacingXDirection;
        private CountdownTimer correctFacingTimer;

        private static readonly int IDLE_HASH = Animator.StringToHash("Idle");
        private static readonly int LOCOMOTION_HASH = Animator.StringToHash("Locomotion");
        private static readonly int CRAWL_IDLE_HASH = Animator.StringToHash("Crawl Idle");
        private static readonly int CRAWL_LOCOMOTION_HASH = Animator.StringToHash("Crawl Locomotion");
        private static readonly int JUMP_HASH = Animator.StringToHash("Jump");
        private static readonly int WARP_JUMP_HASH = Animator.StringToHash("Warp Jump");
        private static readonly int WALL_JUMP_HASH = Animator.StringToHash("Wall Jump");
        private static readonly int WALL_SLIDE_HASH = Animator.StringToHash("Wall Slide");
        private static readonly int BOUNCE_HASH = Animator.StringToHash("Bounce");
        private static readonly int FALL_HASH = Animator.StringToHash("Fall");
        private static readonly int THROW_IDLE_HASH = Animator.StringToHash("Throw Idle");
        private static readonly int THROW_LOCOMOTION_HASH = Animator.StringToHash("Throw Locomotion");

        private EventBinding<PotentiateFeedback> onPotentiateFeedback;
        private EventBinding<ForceDirectionChange> onForceDirectionChange;

        private void OnEnable()
        {
            onPotentiateFeedback = new EventBinding<PotentiateFeedback>(ChangeColor);
            EventBus<PotentiateFeedback>.Register(onPotentiateFeedback);

            onForceDirectionChange = new EventBinding<ForceDirectionChange>(ForceDirectionChange);
            EventBus<ForceDirectionChange>.Register(onForceDirectionChange);
        }

        private void OnDisable()
        {
            EventBus<PotentiateFeedback>.Deregister(onPotentiateFeedback);
            EventBus<ForceDirectionChange>.Deregister(onForceDirectionChange);
        }

        private void Update()
        {
            // Exit case - should not update the facing direction
            if (!updateFacingDirection) return;

            // Check the facing direction of the sprite
            CheckFacingDirection();
        }

        /// <summary>
        /// Initialize the AnimatorController
        /// </summary>
        public void Initialize(PlayerController playerController)
        {
            // Get and set components
            this.playerController = playerController;

            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            // Set variables
            updateFacingDirection = true;

            // Create Countdown Timer
            correctFacingTimer = new CountdownTimer(0.1f);
            correctFacingTimer.OnTimerStart += () => updateFacingDirection = false;
            correctFacingTimer.OnTimerStop += () => updateFacingDirection = true;
        }

        /// <summary>
        /// Check which direction to show the Player facing towards
        /// </summary>
        private void CheckFacingDirection()
        {
            // Create a container for the direction to face
            float directionToFace;

            // Check if wall jumping
            if (playerController.WallJump.IsOnWall)
            {
                directionToFace = playerController.WallJump.WallDirectionThisFrame;

                Vector3 wallScale = transform.localScale;
                wallScale.x = directionToFace;
                transform.localScale = wallScale;

                return;
            }

            // Create a container for the x-velocity
            float xVelocity;
            
            // Check if the player is being forced to move
            if (playerController.ForcedMove)
                // Face towards the movement direction
                xVelocity = playerController.Direction.x;
            else
                // Get an x-velocity from the Player's input
                xVelocity = playerController.FrameData.Input.Move.x;

            // Check if the Player is idle and there's a stored facing direction
            if (xVelocity == 0 && lastFacingXDirection != 0)
                // Set the last facing x-direction
                directionToFace = lastFacingXDirection;
            else
            {
                // Set the direction of input as the facing direction
                directionToFace = Mathf.Sign(xVelocity); ;
                lastFacingXDirection = directionToFace;
            }

            // Apply the direction to face
            Vector3 scale = transform.localScale;
            scale.x = directionToFace;
            transform.localScale = scale;
        }

        /// <summary>
        /// Manually correct the facing direction
        /// </summary>
        public void ForceDirectionChange(ForceDirectionChange eventData)
        {
            // Apply the direction to face
            Vector3 scale = transform.localScale;
            scale.x = eventData.DirectionToFace;
            transform.localScale = scale;

            // Set the last facing direction
            lastFacingXDirection = eventData.DirectionToFace;

            // Exit case - should not buffer the update
            if (!eventData.BufferUpdate) return;

            // Start the correct facing timer
            correctFacingTimer.Start();
        }

        public void ChangeColor(PotentiateFeedback eventData)
        {
            spriteRenderer.color = eventData.Color;
        }

        // Animation Entering Functions
        public void EnterIdle() => animator.CrossFade(IDLE_HASH, crossFadeDuration);
        public void EnterLocomotion() => animator.CrossFade(LOCOMOTION_HASH, crossFadeDuration);
        public void EnterCrawlIdle() => animator.CrossFade(CRAWL_IDLE_HASH, crossFadeDuration);
        public void EnterCrawlLocomotion() => animator.CrossFade(CRAWL_LOCOMOTION_HASH, crossFadeDuration);
        public void EnterJump() => animator.CrossFade(JUMP_HASH, crossFadeDuration);
        public void EnterWarpJump() => animator.CrossFade(WARP_JUMP_HASH, crossFadeDuration);
        public void EnterWallJump() => animator.CrossFade(WALL_JUMP_HASH, crossFadeDuration);
        public void EnterWallSlide() => animator.CrossFade(WALL_SLIDE_HASH, crossFadeDuration);
        public void EnterBounce() => animator.CrossFade(BOUNCE_HASH, crossFadeDuration);
        public void EnterFall() => animator.CrossFade(FALL_HASH, crossFadeDuration);
    }
}

using GoodLuckValley.Architecture.EventBus;
using GoodLuckValley.Player.Movement;
using GoodLuckValley.Potentiates;
using UnityEngine;

namespace GoodLuckValley.Player.Animation
{
    public class AnimationController : MonoBehaviour
    {
        private PlayerController playerController;
        private Animator animator;
        private SpriteRenderer spriteRenderer;
        [SerializeField] private float crossFadeDuration;
        private float lastFacingXDirection;

        private static readonly int IDLE_HASH = Animator.StringToHash("Idle");
        private static readonly int LOCOMOTION_HASH = Animator.StringToHash("Locomotion");
        private static readonly int CRAWL_IDLE_HASH = Animator.StringToHash("Crawl Idle");
        private static readonly int CRAWL_LOCOMOTION_HASH = Animator.StringToHash("Crawl Locomotion");
        private static readonly int JUMP_HASH = Animator.StringToHash("Jump");
        private static readonly int WALL_SLIDE_HASH = Animator.StringToHash("Wall Slide");
        private static readonly int BOUNCE_HASH = Animator.StringToHash("Bounce");
        private static readonly int FALL_HASH = Animator.StringToHash("Fall");
        private static readonly int WALL_JUMP_HASH = Animator.StringToHash("Wall Jump");
        private static readonly int THROW_IDLE_HASH = Animator.StringToHash("Throw Idle");
        private static readonly int THROW_LOCOMOTION_HASH = Animator.StringToHash("Throw Locomotion");

        private EventBinding<PotentiateFeedback> onPotentiateFeedback;

        private void OnEnable()
        {
            // Exit case - there's no PlayerController
            if (playerController == null) return;

            playerController.WallJump.OnWallJump += CorrectFacingDirection;
            EventBus<PotentiateFeedback>.Register(onPotentiateFeedback);
        }

        private void OnDisable()
        {
            // Exit case - there's no PlayerController
            if (playerController == null) return;

            playerController.WallJump.OnWallJump -= CorrectFacingDirection;
            EventBus<PotentiateFeedback>.Deregister(onPotentiateFeedback);
        }

        private void Update()
        {
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

            // Subscribe to events
            playerController.WallJump.OnWallJump += CorrectFacingDirection;

            onPotentiateFeedback = new EventBinding<PotentiateFeedback>(ChangeColor);
            EventBus<PotentiateFeedback>.Register(onPotentiateFeedback);
        }

        /// <summary>
        /// Check which direction to show the Player facing towards
        /// </summary>
        private void CheckFacingDirection()
        {
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

            // Get an x-velocity from the Player's input
            float xVelocity = playerController.FrameData.Input.Move.x;

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
        private void CorrectFacingDirection(int directionToFace)
        {
            // Apply the direction to face
            Vector3 scale = transform.localScale;
            scale.x = directionToFace;
            transform.localScale = scale;

            // Set the last facing direction
            lastFacingXDirection = directionToFace;
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
        public void EnterBounce() => animator.CrossFade(BOUNCE_HASH, crossFadeDuration);
        public void EnterFall() => animator.CrossFade(FALL_HASH, crossFadeDuration);
        public void EnterWallSlide() => animator.CrossFade(WALL_SLIDE_HASH, crossFadeDuration);
        public void EnterWallJump() => animator.CrossFade(WALL_JUMP_HASH, crossFadeDuration);
    }
}

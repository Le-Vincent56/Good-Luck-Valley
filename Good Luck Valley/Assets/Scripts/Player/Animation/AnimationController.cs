using GoodLuckValley.Player.Movement;
using UnityEngine;

namespace GoodLuckValley.Player.Animation
{
    public class AnimationController : MonoBehaviour
    {
        private PlayerController playerController;
        private Animator animator;
        [SerializeField] private float crossFadeDuration;
        private float lastFacingXDirection;

        private static readonly int IDLE_HASH = Animator.StringToHash("Idle");
        private static readonly int LOCOMOTION_HASH = Animator.StringToHash("Locomotion");
        private static readonly int CRAWL_IDLE_HASH = Animator.StringToHash("Crawl Idle");
        private static readonly int CRAWL_LOCOMOTION_HASH = Animator.StringToHash("Crawl Locomotion");
        private static readonly int SLIDE_HASH = Animator.StringToHash("Slide");
        private static readonly int JUMP_HASH = Animator.StringToHash("Jump");
        private static readonly int WALL_SLIDE_HASH = Animator.StringToHash("Wall Slide");
        private static readonly int FALL_HASH = Animator.StringToHash("Fall");
        private static readonly int BOUNCE_HASH = Animator.StringToHash("Bounce");
        private static readonly int WALL_JUMP_HASH = Animator.StringToHash("Wall Jump");
        private static readonly int THROW_IDLE_HASH = Animator.StringToHash("Throw Idle");
        private static readonly int THROW_LOCOMOTION_HASH = Animator.StringToHash("Throw Locomotion");

        private void Update()
        {
            CheckFacingDirection();
        }

        /// <summary>
        /// Initialize the AnimatorController
        /// </summary>
        public void Initialize(PlayerController playerController)
        {
            this.playerController = playerController;
            animator = GetComponent<Animator>();
        }

        /// <summary>
        /// Check which direction to show the Player facing towards
        /// </summary>
        private void CheckFacingDirection()
        {
            float directionToFace;

            // Check if sliding
            if (playerController.Slide.Sliding)
            {
                // If so, face the direction to slide in
                directionToFace = Mathf.Sign(playerController.Collisions.GroundNormal.x);
            } else
            {
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
            }

            // Apply the direction to face
            Vector3 scale = transform.localScale;
            scale.x = directionToFace;
            transform.localScale = scale;
        }

        // Animation Entering Functions
        public void EnterIdle() => animator.CrossFade(IDLE_HASH, crossFadeDuration);
        public void EnterLocomotion() => animator.CrossFade(LOCOMOTION_HASH, crossFadeDuration);
        public void EnterCrawlIdle() => animator.CrossFade(CRAWL_IDLE_HASH, crossFadeDuration);
        public void EnterCrawlLocomotion() => animator.CrossFade(CRAWL_LOCOMOTION_HASH, crossFadeDuration);
        public void EnterSlide() => animator.CrossFade(SLIDE_HASH, crossFadeDuration);
        public void EnterJump() => animator.CrossFade(JUMP_HASH, crossFadeDuration);
        public void EnterFall() => animator.CrossFade(FALL_HASH, crossFadeDuration);
    }
}

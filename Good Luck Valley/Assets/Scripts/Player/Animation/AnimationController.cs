using GoodLuckValley.Events;
using GoodLuckValley.Events.Animation;
using GoodLuckValley.Events.Development;
using GoodLuckValley.Events.Potentiates;
using GoodLuckValley.Events.UI;
using GoodLuckValley.Input;
using GoodLuckValley.Player.Movement;
using GoodLuckValley.Timers;
using UnityEngine;


namespace GoodLuckValley.Player.Animation
{
    public class AnimationController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameInputReader inputReader;
        private PlayerController playerController;
        private Animator animator;
        private SpriteRenderer spriteRenderer;

        [Header("Fields")]
        [SerializeField] private bool active;
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
        private static readonly int SLIDE_HASH = Animator.StringToHash("Slide");

        private EventBinding<SetPaused> onSetPaused;
        private EventBinding<PotentiateFeedback> onPotentiateFeedback;
        private EventBinding<ForceDirectionChange> onForceDirectionChange;
        private EventBinding<ChangeDevelopmentTools> onChangeDevelopmentTools;

        private void OnEnable()
        {
            onSetPaused = new EventBinding<SetPaused>(SetActive);
            EventBus<SetPaused>.Register(onSetPaused);

            onForceDirectionChange = new EventBinding<ForceDirectionChange>(ForceDirectionChange);
            EventBus<ForceDirectionChange>.Register(onForceDirectionChange);

            onChangeDevelopmentTools = new EventBinding<ChangeDevelopmentTools>(SetVisibility);
            EventBus<ChangeDevelopmentTools>.Register(onChangeDevelopmentTools);
        }

        private void OnDisable()
        {
            EventBus<SetPaused>.Deregister(onSetPaused);
            EventBus<ForceDirectionChange>.Deregister(onForceDirectionChange);
            EventBus<ChangeDevelopmentTools>.Deregister(onChangeDevelopmentTools);
        }

        private void Update()
        {
            // Exit case - if not active
            if (!active) return;

            // Exit case - should not update the facing direction
            if (!updateFacingDirection) return;

            // Check the facing direction of the sprite
            CheckFacingDirection();
        }

        /// <summary>
        /// Set whether or not the AnimationController is active
        /// </summary>
        private void SetActive(SetPaused eventData) => active = !eventData.Paused;

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

            // Check if sliding
            if(playerController.Collisions.IsSliding)
            {
                directionToFace = playerController.Collisions.SlideDirection;

                Vector3 slopeScale = transform.localScale;
                slopeScale.x = directionToFace;
                transform.localScale = slopeScale;

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
        /// Rotate the Player to align with the ground
        /// </summary>
        public void RotatePlayer()
        {
            // Get the normal of the surface
            Vector2 normal = playerController.Collisions.GroundNormal;

            // Calculate the angle (in degrees) from the normal to the upward direction
            float angle = Mathf.Atan2(normal.y, normal.x) * Mathf.Rad2Deg - 90f;

            // Apply the rotation to the sprite
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        /// <summary>
        /// Correct the Player rotation
        /// </summary>
        public void CorrectPlayerRotation()
        {
            transform.rotation = Quaternion.identity;
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

        /// <summary>
        /// Correct the Player sprite on a slope
        /// </summary>
        public void CorrectPlayerSlope()
        {
            // Get the normal of the surface
            //float angle = playerController.Collisions.LastSteepSlopeAngle - 45f;
            float angle = 45f - playerController.Collisions.LastSteepSlopeAngle;

            // Apply the rotation to the sprite
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        /// <summary>
        /// Set the visibility of the player sprite
        /// </summary>
        private void SetVisibility(ChangeDevelopmentTools eventData)
        {
            spriteRenderer.enabled = !eventData.Invisible;
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
        public void EnterSlide() => animator.CrossFade(SLIDE_HASH, crossFadeDuration);
    }
}

using GoodLuckValley.Timers;
using System;
using UnityEngine;

namespace GoodLuckValley.Player.Movement
{
    [Serializable]
    public class PlayerBounce
    {
        private PlayerController controller;
        [SerializeField] private bool bouncing;
        [SerializeField] private bool bouncePrepped;
        [SerializeField] private bool canSlowFall;
        [SerializeField] private bool canDetectGround;
        private float yContactValue;
        private CountdownTimer ignoreDetectionTimer;

        public bool Bouncing { get => bouncing || bouncePrepped; set => bouncing = value; }
        public bool FromBounce { get => bouncing || bouncePrepped || canSlowFall; }
        public bool CanSlowFall { get => canSlowFall; set => canSlowFall = value; }
        public bool CanDetectGround { get => canDetectGround; }

        public PlayerBounce(PlayerController controller)
        {
            this.controller = controller;

            canDetectGround = true;

            // Create the timer
            ignoreDetectionTimer = new CountdownTimer(controller.Stats.BounceIgnoreDetectionTime);

            // Set timer callbacks
            ignoreDetectionTimer.OnTimerStart += () => canDetectGround = false;
            ignoreDetectionTimer.OnTimerStop += () => canDetectGround = true;
        }

        /// <summary>
        /// Calculate whether the Player should still be bouncing
        /// </summary>
        public void CalculateBounce()
        {
            // Check if not bouncing, but a bounce is prepped
            if (!bouncing && bouncePrepped)
            {
                // Set variables
                bouncing = true;
                bouncePrepped = false;

                // Calculate the bounce force based on yContactValue
                float bouncePower = Mathf.Lerp(controller.Stats.MaxBouncePower, controller.Stats.MinBouncePower, yContactValue);

                // Add the bounce force to the player
                controller.FrameData.AddForce(new Vector2(0, bouncePower), true, true);
            }

            // Check if bouncing and descending
            else if (bouncing && controller.Velocity.y < 0f)
            {
                // Stop bouncing
                bouncing = false;

                // Allow slow falling
                canSlowFall = true;
            }
        }

        /// <summary>
        /// Prepare variables for the bounce
        /// </summary>
        public void PrepareBounce(float yContactValue)
        {
            // Exit case - already a bounce prepped or the ignore detection timer is runn
            if (bouncePrepped || !canDetectGround) return;

            bouncePrepped = true;
            this.yContactValue = yContactValue;

            // Start the ignore detection timer
            ignoreDetectionTimer.Start();

            // Set not-grounded
            controller.Collisions.ToggleGrounded(false);
        }

        /// <summary>
        /// Reset bounce variables
        /// </summary>
        public void ResetBounce(bool canSlowFall = false)
        {
            bouncing = false;
            bouncePrepped = false;
            this.canSlowFall = canSlowFall;
        }
    }
}

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
            // Check if bouncing and descending
            if (bouncing)
            {
                // Stop bouncing
                canSlowFall = true;
            }

            // Check if not bouncing, but a bounce is prepped
            if(!bouncing && bouncePrepped)
            {
                // Execute the bounce
                bouncing = true;

                // Add the bounce force to the player
                controller.FrameData.AddForce(new Vector2(0, controller.Stats.BouncePower), true, true);
            }
        }

        /// <summary>
        /// Prepare variables for the bounce
        /// </summary>
        public void PrepareBounce()
        {
            // Exit case - already a bounce prepped
            if (bouncePrepped) return;

            bouncePrepped = true;

            // Start the ignore detection timer
            ignoreDetectionTimer.Start();

            // Set not-grounded
            controller.Collisions.ToggleGrounded(false);
        }

        /// <summary>
        /// Reset bounce variables
        /// </summary>
        public void ResetBounce()
        {
            bouncing = false;
            bouncePrepped = false;
        }
    }
}

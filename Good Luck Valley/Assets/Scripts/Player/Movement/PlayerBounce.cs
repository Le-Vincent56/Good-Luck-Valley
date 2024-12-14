using GoodLuckValley.Timers;
using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
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

        public bool Bouncing { get => bouncing; set => bouncing = value; }
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
            if (bouncing && controller.RB.linearVelocity.y < 0 && !controller.Collisions.Grounded)
            {
                // Stop bouncing
                canSlowFall = true;
            }
            // Otherwise check if not bouncing, but a bounce is prepped
            else if(!bouncing && bouncePrepped)
            {
                // Execute the bounce
                bouncing = true;
                ExecuteBounce();
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
        }

        /// <summary>
        /// Execute the bounce
        /// </summary>
        public void ExecuteBounce()
        {
            // Prepare the controller variables
            controller.SetVelocity(controller.FrameData.TrimmedVelocity);

            // Start the ignore detection timer
            ignoreDetectionTimer.Reset();
            ignoreDetectionTimer.Start();

            // Set not-grounded
            controller.Collisions.ToggleGrounded(false);

            // Add the bounce force to the player
            controller.FrameData.AddForce(new Vector2(0, controller.Stats.BouncePower));
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

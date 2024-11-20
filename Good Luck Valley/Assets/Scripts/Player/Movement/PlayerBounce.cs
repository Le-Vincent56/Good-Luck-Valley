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
        [SerializeField] private bool canSlowFall;

        public bool Bouncing { get => bouncing; set => bouncing = value; }
        public bool CanSlowFall { get => canSlowFall; set => canSlowFall = value; }

        public PlayerBounce(PlayerController controller)
        {
            this.controller = controller;
        }

        /// <summary>
        /// Calculate whether the Player should still be bouncing
        /// </summary>
        public void CalculateBounce()
        {
            // Check if bouncing and descending
            if (bouncing && controller.RB.velocity.y < 0 && !controller.Collisions.Grounded)
            {
                // Stop bouncing
                bouncing = false;
                canSlowFall = true;
            }
        }

        /// <summary>
        /// Prepare variables for the bounce
        /// </summary>
        public void PrepareBounce()
        {
            // Exit case - already bouncing
            if (bouncing) return;

            controller.SetVelocity(controller.FrameData.TrimmedVelocity);
            controller.RB.gravityScale = controller.InitialGravityScale;
            controller.ExtraConstantGravity = controller.Stats.BounceConstantGravity;
            bouncing = true;

            // Add the bounce force to the player
            controller.FrameData.AddForce(new Vector2(0, controller.Stats.BouncePower));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine.States
{
    public class PlayerBounceState : PlayerAbilityState
    {
        #region FIELDS
        private bool isBouncing;
        private float bounceBuffer;
        private float bounceTimer;
        #endregion

        #region PROPERTIES
        public bool Rotated { get; set; }
        public Vector2 BounceVector { get; set; }
        public ForceMode2D ForceMode { get; set; }
        #endregion

        public PlayerBounceState(PlayerController player, PlayerStateMachine stateMachine, PlayerData playerData, string animationBoolName)
                : base(player, stateMachine, playerData, animationBoolName)
        {
            bounceBuffer = 0.5f;
        }

        public override void Enter()
        {
            base.Enter();

            // Enter the bounce
            if (!isBouncing)
            {
                // Set bouncing
                isBouncing = true;
                bounceTimer = bounceBuffer;

                // Set gravity multiplier
                player.SetGravityScale(playerData.gravityScale);

                // Clear the velocity
                player.RB.velocity = Vector2.zero;

                // Add the force
                player.RB.AddForce(BounceVector, ForceMode);
            }
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Check if the bounce is over
            if (isBouncing && player.RB.velocity.y < 0.01f)
            {
                // End the jump
                EndBounce();
            }

            if (!isGrounded && isOnWall) // Exit case - end early if on wall
            {
                // End the jump
                EndBounce();

                // Change to the wall slide state
                stateMachine.ChangeState(player.WallSlideState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            // If rotated, don't allow movement
            if (Rotated) return;

            // Move the player in the air
            player.Move(0.5f, false, true);
        }

        /// <summary>
        /// End the bounce
        /// </summary>
        public void EndBounce()
        {
            // Set bouncing to false
            isBouncing = false;

            // End bounce
            player.EndBounce();

            // Trigger ability done
            isAbilityDone = true;
        }
    }
}

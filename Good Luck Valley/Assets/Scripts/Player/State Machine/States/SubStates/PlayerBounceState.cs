using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine.States
{
    public class PlayerBounceState : PlayerAbilityState
    {
        #region FIELDS
        private bool isBouncing;
        #endregion

        public PlayerBounceState(PlayerController player, PlayerStateMachine stateMachine, PlayerData playerData, string animationBoolName)
                : base(player, stateMachine, playerData, animationBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            // Enter the bounce
            if (!isBouncing)
            {
                // Set bouncing
                isBouncing = true;

                // Set gravity multiplier
                player.SetGravityScale(playerData.gravityScale);
            }
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Check if the bounce is over
            if (isBouncing && player.RB.velocity.y < 0.01f)
            {
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

            // Move the player in the air
            player.Move(0.5f, true, true);
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

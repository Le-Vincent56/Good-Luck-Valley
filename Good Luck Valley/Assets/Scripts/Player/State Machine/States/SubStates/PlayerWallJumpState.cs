using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine.States
{
    public class PlayerWallJumpState : PlayerAbilityState
    {
        #region FIELDS
        private bool isWallJumping;
        #endregion

        public PlayerWallJumpState(PlayerControllerOld player, PlayerStateMachine stateMachine, PlayerData playerData, string animationBoolName) 
            : base(player, stateMachine, playerData, animationBoolName)
        {
        }
        public override void Enter()
        {
            base.Enter();

            // Enter the bounce
            if (!isWallJumping)
            {
                // Set bouncing
                isWallJumping = true;

                // Set gravity multiplier
                player.SetGravityScale(playerData.gravityScale);
            }
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Check if the bounce is over
            if (isWallJumping && player.RB.velocity.y < 0.01f)
            {
                EndWallJump();
            }

            if (!isGrounded && isOnWall) // Exit case - end early if on wall
            {
                // If the previous state is the wall state, don't end the jump, as we have just left the wall
                if (stateMachine.PreviousState is PlayerWallState) return;

                // End the jump
                EndWallJump();

                // Change to the wall slide state
                stateMachine.ChangeState(player.WallSlideState);
            }
        }

        /// <summary>
        /// End the Wall Jump
        /// </summary>
        public void EndWallJump()
        {
            // Set bouncing to false
            isWallJumping = false;

            // End bounce
            player.EndWallJump();

            // Trigger ability done
            isAbilityDone = true;
        }
    }
}

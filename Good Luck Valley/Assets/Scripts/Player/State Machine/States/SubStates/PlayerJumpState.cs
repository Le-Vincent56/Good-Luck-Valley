using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine.States
{
    public class PlayerJumpState : PlayerAbilityState
    {
        private bool isJumping;
        private bool isJumpCut;

        public PlayerJumpState(PlayerController player, PlayerStateMachine stateMachine, PlayerData playerData, string animationBoolName) 
            : base(player, stateMachine, playerData, animationBoolName)
        {
        }

        public override void DoChecks()
        {
            base.DoChecks();
        }

        public override void Enter()
        {
            base.Enter();

            if(!isJumping)
            {
                isJumping = true;
                isJumpCut = false;

                player.InputHandler.LastPressedJumpTime = 0;

                player.SetGravityScale(playerData.gravityScale);

                Jump();
            }
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Update timers
            player.InputHandler.LastPressedJumpTime -= Time.deltaTime;

            // Check for input changes - jump cut
            if (player.InputHandler.TryJumpCut)
            {
                // Check if the player is jumping and going upwards
                if(isJumping && player.RB.velocity.y > 0)
                {
                    // If so, they can jump cut
                    isJumpCut = true;
                }
            }

            // Gravity alterations
            if (isJumpCut) // If jump cutting
            {
                // Higher gravity if jump button released
                player.SetGravityScale(playerData.gravityScale * playerData.jumpCutGravityMult);
                player.RB.velocity = new Vector2(player.RB.velocity.x, Mathf.Max(player.RB.velocity.y, -playerData.maxFallSpeed));
            }
            else if (isJumping && Mathf.Abs(player.RB.velocity.y) < playerData.jumpHangTimeThreshold) // If jump hanging
            {
                player.SetGravityScale(playerData.gravityScale * playerData.jumpHangGravityMult);
            }

            // Check if the jump has ended
            if (isJumping && player.RB.velocity.y < 0.01f)
            {
                EndJump();
            }

            if (!isGrounded && isOnWall) // Exit case - end early if on wall
            {
                // End the jump
                EndJump();

                // Change to the wall slide state
                stateMachine.ChangeState(player.WallSlideState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            // Move the player in the air
            player.Move(0.5f, true, false);
        }

        /// <summary>
        /// Add jumping force
        /// </summary>
        public void Jump()
        {
            float force = playerData.jumpForce;
            if (player.RB.velocity.y < 0)
            {
                force -= player.RB.velocity.y;
            }

            player.RB.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        }

        /// <summary>
        /// End the jump
        /// </summary>
        public void EndJump()
        {
            // Set is jumping to false
            isJumping = false;

            // Set jump cut to false
            isJumpCut = false;

            // Reset the jump input variable
            player.InputHandler.UseJumpInput();

            // Trigger ability done
            isAbilityDone = true;
        }
    }
}

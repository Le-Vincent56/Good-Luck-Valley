using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine.States
{
    public class PlayerGroundedState : PlayerState
    {
        protected float lastOnGroundTime;
        protected int xInput;
        private bool jumpInput;
        private bool isGrounded;

        public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animationaBoolName) 
            : base(player, stateMachine, playerData, animationaBoolName)
        {
        }

        public override void DoChecks()
        {
            base.DoChecks();

            isGrounded = player.CheckIfGrounded();
        }

        public override void Enter()
        {
            base.Enter();

            // Set coyote time
            lastOnGroundTime = playerData.coyoteTime;

            // Set gravity
            player.SetGravityScale(playerData.gravityScale);
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Read in the input from the input handler
            xInput = player.InputHandler.NormInputX;
            jumpInput = player.InputHandler.JumpInput;

            // Exit case - Jumping
            if(jumpInput && lastOnGroundTime > 0 && player.InputHandler.LastPressedJumpTime > 0)
            {
                // Change states
                stateMachine.ChangeState(player.JumpState);
            } else if(!isGrounded) // Exit case - in the air
            {
                stateMachine.ChangeState(player.InAirState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}


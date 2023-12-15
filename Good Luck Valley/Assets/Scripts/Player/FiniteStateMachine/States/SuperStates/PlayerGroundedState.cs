using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine.States
{
    public class PlayerGroundedState : PlayerState
    {
        protected int xInput;
        private bool jumpInput;

        public PlayerGroundedState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animationaBoolName) 
            : base(player, stateMachine, playerData, animationaBoolName)
        {
        }

        public override void DoChecks()
        {
            base.DoChecks();
        }

        public override void Enter()
        {
            base.Enter();
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
            if(jumpInput)
            {
                // Reset the jump input variable
                player.InputHandler.UseJumpInput();

                // Change states
                stateMachine.ChangeState(player.JumpState);
                player.JumpState.LastOnGroundTime = playerData.coyoteTime;
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}


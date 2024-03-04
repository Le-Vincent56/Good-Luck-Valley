using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine.States
{
    public class PlayerInAirState : PlayerState
    {
        private bool isGrounded;
        private int xInput;

        public PlayerInAirState(PlayerController player, PlayerStateMachine stateMachine, PlayerData playerData, string animationaBoolName) 
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
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Update x input 
            xInput = player.InputHandler.NormInputX;

            // Exit case - player is grounded
            if(isGrounded && player.RB.velocity.y < 0.01f)
            {
                stateMachine.ChangeState(player.LandState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            if(!(isGrounded && player.RB.velocity.y < 0.01f))
            {
                player.Move(0.5f, true);

                // Set animations
                player.Anim.SetFloat("yVelocity", player.RB.velocity.y);
                player.Anim.SetFloat("xVelocity", Mathf.Abs(player.RB.velocity.x));
            }
        }
    }
}
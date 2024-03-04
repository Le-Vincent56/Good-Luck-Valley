using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine.States 
{
    public class PlayerAbilityState : PlayerState
    {
        protected bool isAbilityDone;
        private bool isGrounded;

        public PlayerAbilityState(PlayerController player, PlayerStateMachine stateMachine, PlayerData playerData, string animationaBoolName) 
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

            isAbilityDone = false;
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Check if the ability is done
            if(isAbilityDone)
            {
                // Check if the player is grounded
                if(isGrounded && player.RB.velocity.y < 0.01f)
                {
                    // If so, change to Idle
                    stateMachine.ChangeState(player.IdleState);
                } else
                {
                    // If not, change to InAir
                    stateMachine.ChangeState(player.InAirState);
                }
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }
    }
}


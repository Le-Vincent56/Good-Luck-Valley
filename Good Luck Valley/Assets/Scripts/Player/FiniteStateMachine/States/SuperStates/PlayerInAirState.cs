using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine.States
{
    public class PlayerInAirState : PlayerState
    {
        #region FIELDS
        private bool isGrounded;
        private bool isBouncing;
        private bool fastFallInput;
        #endregion

        public PlayerInAirState(PlayerController player, PlayerStateMachine stateMachine, PlayerData playerData, string animationBoolName) 
            : base(player, stateMachine, playerData, animationBoolName)
        {
        }

        public override void DoChecks()
        {
            base.DoChecks();

            isGrounded = player.CheckIfGrounded();
            isBouncing = player.CheckIfBouncing();
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

            // Get the fast fall input
            fastFallInput = player.InputHandler.FastFallInput;

            // Check for fall vs. fast fall
            if(fastFallInput)
            {
                stateMachine.ChangeState(player.FastFallState);
            } else
            {
                stateMachine.ChangeState(player.FallState);
            }


            if (isBouncing) // Exit case - bouncing
            {
                stateMachine.ChangeState(player.BounceState);
            }
            else if (isGrounded) // Exit case - player is grounded
            {
                stateMachine.ChangeState(player.LandState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            // Move the player in the air
            player.Move(0.5f, true);
        }
    }
}
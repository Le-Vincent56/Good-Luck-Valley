using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine.States
{
    public class PlayerInAirState : PlayerState
    {
        #region FIELDS
        private float xInput;
        private bool isGrounded;
        private bool isBouncing;
        private bool isOnWall;
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
            isOnWall = player.CheckIfWalled();
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

            // Get input
            xInput = player.InputHandler.NormInputX;
            fastFallInput = player.InputHandler.FastFallInput;

            // Check for fall vs. fast fall (and if the player is not already in that respective state)
            if(fastFallInput && stateMachine.CurrentState is not PlayerFastFallState)
            {
                stateMachine.ChangeState(player.FastFallState);
            } else if(stateMachine.CurrentState is not PlayerFallState)
            {
                stateMachine.ChangeState(player.FallState);
            }

            if (isBouncing) // Exit case - bouncing
            {
                stateMachine.ChangeState(player.BounceState);
            }
            else if(!isGrounded && isOnWall) // Exit case - player is on a wall
            {
                stateMachine.ChangeState(player.WallSlideState);
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
            player.Move(0.5f, true, false);
        }
    }
}
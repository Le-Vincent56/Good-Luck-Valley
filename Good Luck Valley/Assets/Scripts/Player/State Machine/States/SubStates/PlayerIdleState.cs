using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine.States
{
    public class PlayerIdleState : PlayerGroundedState
    {
        public PlayerIdleState(PlayerControllerOld player, PlayerStateMachine stateMachine, PlayerData playerData, string animationaBoolName) 
            : base(player, stateMachine, playerData, animationaBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            //// Set gravity to 0 and stop player movement
            //if(stateMachine.PreviousState is PlayerSlopeState)
            //{
            //    player.SetGravityScale(0f);
            //    player.RB.velocity = Vector2.zero;
            //}
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Has a chance of cancelling jump, so we need guard clauses
            if(isOnSlope && (stateMachine.CurrentState is not PlayerJumpState && stateMachine.PreviousState is not PlayerJumpState))
            {
                player.SetGravityScale(0f);
                player.RB.velocity = Vector2.zero;
            }

            // Exit case - player is moving and not paused
            if(xInput != 0f && !player.Paused)
            {
                stateMachine.ChangeState(player.MoveState);
            }
        }
    }
}

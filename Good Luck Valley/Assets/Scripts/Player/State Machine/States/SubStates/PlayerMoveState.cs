using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine.States
{
    public class PlayerMoveState : PlayerGroundedState
    {
        public PlayerMoveState(PlayerControllerOld player, PlayerStateMachine stateMachine, PlayerData playerData, string animationaBoolName) 
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

            if (isOnSlope) // Exit case - entered slope
            {
                stateMachine.ChangeState(player.SlopeState);
            }
            else if (xInput == 0f && !player.Paused) // Exit case - player is idle and not paused
            {
                stateMachine.ChangeState(player.IdleState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            // Move the player
            player.Move(0.5f);
        }
    }
}

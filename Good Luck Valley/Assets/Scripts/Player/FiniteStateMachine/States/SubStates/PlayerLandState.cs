using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine.States
{
    public class PlayerLandState : PlayerGroundedState
    {
        public PlayerLandState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animationaBoolName) : base(player, stateMachine, playerData, animationaBoolName)
        {
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Exit case - player is moving
            if(xInput != 0)
            {
                stateMachine.ChangeState(player.MoveState);
            } else if(isAnimationFinished) // Exit case - land animation finished
            {
                stateMachine.ChangeState(player.IdleState);
            }
        }
    }
}

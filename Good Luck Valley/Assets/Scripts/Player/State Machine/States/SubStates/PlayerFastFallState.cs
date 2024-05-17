using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine.States
{
    public class PlayerFastFallState : PlayerInAirState
    {
        public PlayerFastFallState(PlayerController player, PlayerStateMachine stateMachine, PlayerData playerData, string animationBoolName)
            : base(player, stateMachine, playerData, animationBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            // Set gravity multiplier
            player.SetGravityScale(playerData.gravityScale * playerData.fastFallGravityMult);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Clamp fall velocity
            player.RB.velocity = new Vector2(player.RB.velocity.x, Mathf.Max(player.RB.velocity.y, -playerData.maxFastFallSpeed));
        }
    }
}

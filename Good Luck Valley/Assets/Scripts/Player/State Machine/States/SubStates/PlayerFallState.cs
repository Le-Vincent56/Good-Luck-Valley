using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

namespace GoodLuckValley.Player.StateMachine.States
{
    public class PlayerFallState : PlayerInAirState
    {
        public PlayerFallState(PlayerController player, PlayerStateMachine stateMachine, PlayerData playerData, string animationBoolName)
            : base(player, stateMachine, playerData, animationBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            // Set gravity multipliers based on previous state
            if (stateMachine.PreviousState is PlayerJumpState) player.SetGravityScale(playerData.gravityScale * playerData.fallGravityMult);
            if (stateMachine.PreviousState is PlayerFastFallState) player.SetGravityScale(playerData.gravityScale * playerData.fallGravityMult);
            if (stateMachine.PreviousState is PlayerBounceState) player.SetGravityScale(playerData.gravityScale * playerData.fallFromBounceGravityMult);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Clamp fall velocity
            if(!player.CheckIfNoClip()) player.RB.velocity = new Vector2(player.RB.velocity.x, Mathf.Max(player.RB.velocity.y, -playerData.maxFallSpeed));
        }
    }
}

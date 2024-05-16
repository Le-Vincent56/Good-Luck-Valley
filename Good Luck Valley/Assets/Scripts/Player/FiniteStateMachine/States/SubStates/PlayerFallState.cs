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
    }
}

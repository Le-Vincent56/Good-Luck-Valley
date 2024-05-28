using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine.States
{
    public class PlayerWallSlideState : PlayerWallState
    {
        public PlayerWallSlideState(PlayerControllerOld player, PlayerStateMachine stateMachine, PlayerData playerData, string animationBoolName)
            : base(player, stateMachine, playerData, animationBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            // If the player was in jump or bounce, they were heading upwards, so use upwards slide gravity
            if (stateMachine.PreviousState is PlayerJumpState || stateMachine.PreviousState is PlayerBounceState)
                player.SetGravityScale(playerData.gravityScale * playerData.wallSlideGravityMultUp);
            else // Otherwise, use downward slide gravity
                player.SetGravityScale(playerData.gravityScale * playerData.wallSlideGravityMultDown);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Clamp slide velocity
            player.RB.velocity = new Vector2(player.RB.velocity.x, Mathf.Max(player.RB.velocity.y, -playerData.maxWallSlideSpeed));
        }
    }
}

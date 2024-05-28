using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine.States
{
    public class PlayerFastWallSlideState : PlayerWallState
    {
        public PlayerFastWallSlideState(PlayerControllerOld player, PlayerStateMachine stateMachine, PlayerData playerData, string animationBoolName) 
            : base(player, stateMachine, playerData, animationBoolName)
        {
        }

        public override void Enter()
        {
            base.Enter();

            // Set gravity scale
            player.SetGravityScale(playerData.gravityScale * playerData.fastWallSlideGravityMultDown);
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Clamp slide velocity
            player.RB.velocity = new Vector2(player.RB.velocity.x, Mathf.Max(player.RB.velocity.y, -playerData.maxFastWallSlideSpeed));
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine.States
{
    public class PlayerJumpState : PlayerAbilityState
    {
        private float lastOnGroundTime;
        private float lastPressedJumpTime;
        private bool isJumping;
        private bool isJumpCut;
        private bool isJumpFalling;

        public float LastOnGroundTime { get { return lastOnGroundTime; } set { lastOnGroundTime = value; } }

        public PlayerJumpState(Player player, PlayerStateMachine stateMachine, PlayerData playerData, string animationaBoolName) 
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

            isJumping = true;
            isJumpCut = false;
            isJumpFalling = false;

            lastPressedJumpTime = 0;

            player.Jump();
        }

        public override void Exit()
        {
            base.Exit();

            
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            lastOnGroundTime -= Time.deltaTime;
            lastPressedJumpTime -= Time.deltaTime;

            // Jump Checks
            if (isJumping && player.RB.velocity.y < 0)
            {
                isJumping = false;

                // Trigger ability done
                isAbilityDone = true;
            }

            // Gravity alterations
            if (isJumpCut) // If jump cutting
            {
                // Higher gravity if jump button released
                player.SetGravityScale(playerData.gravityScale * playerData.jumpCutGravityMult);
                player.RB.velocity = new Vector2(player.RB.velocity.x, Mathf.Max(player.RB.velocity.y, -playerData.maxFallSpeed));
            }
            else if ((isJumping || isJumpFalling) && Mathf.Abs(player.RB.velocity.y) < playerData.jumpHangTimeThreshold) // If jump hanging
            {
                player.SetGravityScale(playerData.gravityScale * playerData.jumpHangGravityMult);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();
        }

        public bool CheckCanJump()
        {
            return lastOnGroundTime > 0 && !isJumping && player.isLocked;
        }
    }
}

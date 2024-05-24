using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Player.StateMachine.States
{
    public class PlayerGroundedState : PlayerState
    {
        #region FIELDS
        protected float lastOnGroundTime;
        protected int xInput;
        private bool jumpInput;
        protected bool isGrounded;
        private bool isBouncing;
        private bool isOnWall;
        private bool isOnTopOfWall;
        protected bool isOnSlope;
        #endregion

        public PlayerGroundedState(PlayerController player, PlayerStateMachine stateMachine, PlayerData playerData, string animationBoolName) 
            : base(player, stateMachine, playerData, animationBoolName)
        {
        }

        public override void DoChecks()
        {
            base.DoChecks();

            isGrounded = player.CheckIfGrounded() || player.CheckIfGroundedLine();
            isBouncing = player.CheckIfBouncing();
            isOnWall = player.CheckIfWalled();
            isOnTopOfWall = player.CheckIfOnTopOfMushWall();
            isOnSlope = player.CheckIfOnSlope();
        }

        public override void Enter()
        {
            base.Enter();

            // Set coyote time
            lastOnGroundTime = playerData.coyoteTime;

            // Set gravity
            player.SetGravityScale(playerData.gravityScale);
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Read in the input from the input handler
            xInput = player.InputHandler.NormInputX;
            jumpInput = player.InputHandler.JumpInput;

            if (!isGrounded && !isOnSlope && !isOnTopOfWall) // Exit case is not grounded
            {
                stateMachine.ChangeState(player.FallState);
            }
            else if (!isGrounded && isOnWall) // Exit case - on wall and not grounded
            {
                stateMachine.ChangeState(player.WallSlideState);
            }
            else if(isBouncing) // Exit case - bouncing
            {
                stateMachine.ChangeState(player.BounceState);
            }
            else if (jumpInput && lastOnGroundTime > 0 && player.InputHandler.LastPressedJumpTime > 0) // Exit case - Jumping
            {
                stateMachine.ChangeState(player.JumpState);
            }
        }
    }
}


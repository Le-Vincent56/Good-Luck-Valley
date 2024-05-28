using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;

namespace GoodLuckValley.Player.StateMachine.States
{
    public class PlayerWallState : PlayerState
    {
        #region FIELDS
        protected int xInput;
        private float wallDirection;
        private bool isBouncing;
        private bool isGrounded;
        private bool isWallJumping;
        private bool fastSlideInput;
        private bool isOnWall;
        #endregion

        public PlayerWallState(PlayerControllerOld player, PlayerStateMachine stateMachine, PlayerData playerData, string animationBoolName) 
            : base(player, stateMachine, playerData, animationBoolName)
        {
        }

        public override void DoChecks()
        {
            base.DoChecks();

            isGrounded = player.CheckIfGrounded();
            isOnWall = player.CheckIfWalled();
            isWallJumping = player.CheckIfWallJumping();
        }
        public override void Enter()
        {
            base.Enter();

            // Store the wall direction
            wallDirection = player.CheckWallDirection();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override void LogicUpdate()
        {
            base.LogicUpdate();

            // Get input variables
            xInput = player.InputHandler.NormInputX;
            fastSlideInput = player.InputHandler.FastFallInput;

            // Check if the player is fast sliding and not already in fast sliding
            if (fastSlideInput)
            {
                stateMachine.ChangeState(player.FastWallSlideState);
            }
            else // Don't repeat
            {
                stateMachine.ChangeState(player.WallSlideState);
            }

            if (isWallJumping) // Exit case - perform wall jump
            {
                stateMachine.ChangeState(player.WallJumpState);
            }
            else if (!isOnWall && !isGrounded) // Exit case - if not on a wall and not on ground, then fall
            {
                stateMachine.ChangeState(player.FallState);
            }
            else if (isBouncing) // Exit case - bouncing
            {
                stateMachine.ChangeState(player.BounceState);
            }
            else if (isGrounded) // Exit case - grounded
            {
                stateMachine.ChangeState(player.LandState);
            }
        }

        public override void PhysicsUpdate()
        {
            base.PhysicsUpdate();

            // Apply movement if there's x-input and not moving
            // against the wall
            if(xInput != 0f && xInput != wallDirection)
            {
                player.Move(0.5f, true);
            }
        }
    }
}

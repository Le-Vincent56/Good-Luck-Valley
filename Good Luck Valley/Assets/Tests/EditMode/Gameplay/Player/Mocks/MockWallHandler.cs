using UnityEngine;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Interfaces;

namespace GoodLuckValley.Tests.EditMode.Gameplay.Player.Mocks
{
    public class MockWallHandler : IWallHandler
    {
        public bool IsOnWall { get; set; }
        public int WallDirection { get; set; }
        public bool IsWallJumping { get; set; }
        public bool HasWallCoyoteTime { get; set; }

        public int ExecuteWallJumpCount { get; private set; }
        public int DetachFromWallCount { get; private set; }
        public int NotifyGroundedCount { get; private set; }

        public bool ShouldAttachResult { get; set; }

        public bool ShouldAttachToWall(
            CollisionData collision, 
            Vector2 moveInput, 
            bool isGrounded
        )
        {
            return ShouldAttachResult;
        }

        public Vector2 AdjustMoveInput(Vector2 rawInput) => rawInput;

        public void AttachToWall(int direction)
        {
            IsOnWall = true;
            WallDirection = direction;
        }

        public void DetachFromWall()
        {
            IsOnWall = false;
            DetachFromWallCount++;
        }

        public void ExecuteWallJump() => ExecuteWallJumpCount++;

        public float CalculateWallVelocity(
            Vector2 moveInput, 
            float currentYVelocity, 
            float deltaTime
        )
        {
            return -1f;
        }

        public void NotifyGrounded() => NotifyGroundedCount++;

        public void Tick(float deltaTime, float currentTime) { 
        }
    }
}
using UnityEngine;
using GoodLuckValley.Gameplay.Player.Data;

namespace GoodLuckValley.Gameplay.Player.Interfaces
{
    /// <summary>
    /// Provides an interface for handling player interactions with walls, including wall detection,
    /// attachment, detachment, and wall-related movement mechanics.
    /// </summary>
    public interface IWallHandler
    {
        bool IsOnWall { get; }
        int WallDirection { get; }
        bool IsWallJumping { get; }
        bool HasWallCoyoteTime { get; }

        bool ShouldAttachToWall(
            CollisionData collision, 
            Vector2 moveInput, 
            bool isGrounded
        );
        Vector2 AdjustMoveInput(Vector2 rawInput);
        void AttachToWall(int direction);
        void DetachFromWall();
        void ExecuteWallJump();
        float CalculateWallVelocity(
            Vector2 moveInput, 
            float currentYVelocity, 
            float deltaTime
        );
        void NotifyGrounded();
        void Tick(float deltaTime, float currentTime);
    }
}
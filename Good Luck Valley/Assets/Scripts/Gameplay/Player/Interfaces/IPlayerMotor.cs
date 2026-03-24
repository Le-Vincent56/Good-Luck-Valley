using UnityEngine;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Motor;

namespace GoodLuckValley.Gameplay.Player.Interfaces
{
    /// <summary>
    /// Interface defining the core functionality of a player motor component,
    /// responsible for handling movement, gravity, and collision-based adjustments
    /// in relation to the player's velocity and state.
    /// </summary>
    public interface IPlayerMotor
    {
        void SetGravityScale(float scale);
        void SetMaxFallSpeed(float maxSpeed);
        void ClearMaxFallSpeed();
        void SetMovementMode(MovementMode mode);
        void ApplyImpulse(Vector2 impulse, bool resetVelocityX, bool resetVelocityY);
        void SetEndJumpEarlyMultiplier(float multiplier);
        void ClearEndJumpEarlyMultiplier();
        void SetWallVelocityOverride(float yVelocity);
        void ClearWallVelocityOverride();
        void BeginTick();
        MotorOutput CalculateVelocity(
            Vector2 currentVelocity, 
            Vector2 moveInput,
            CollisionData collision, 
            float globalGravityY, 
            float deltaTime
        );
    }
}
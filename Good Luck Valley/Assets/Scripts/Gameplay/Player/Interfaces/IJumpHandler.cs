using GoodLuckValley.Gameplay.Player.Data;

namespace GoodLuckValley.Gameplay.Player.Interfaces
{
    /// <summary>
    /// Defines methods and properties to manage jump mechanics for a player in the game.
    /// </summary>
    public interface IJumpHandler
    {
        bool IsInJumpClearance { get; }
        bool EndedJumpEarly { get; }
        int AirJumpsRemaining { get; }
        JumpType LastJumpType { get; }
        bool HasCoyoteTime { get; }
        bool HasAirJump { get; }

        void ExecuteNormalJump();
        void ExecuteAirJump();
        void NotifyGrounded();
        void NotifyLeftGround();
        void NotifyJumpExecuted(JumpType type);
        void NotifyJumpReleased();
        void RestoreAirJump();
        void GrantAirJumps(int count);
        void Tick(float deltaTime, float currentTime);
    }
}
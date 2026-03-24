using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Interfaces;

namespace GoodLuckValley.Tests.EditMode.Gameplay.Player.Mocks
{
    public class MockJumpHandler : IJumpHandler
    {
        public bool IsInJumpClearance { get; set; }
        public bool EndedJumpEarly { get; set; }
        public int AirJumpsRemaining { get; set; }
        public JumpType LastJumpType { get; set; }
        public bool HasCoyoteTime { get; set; }
        public bool HasAirJump { get; set; }

        public int ExecuteNormalJumpCount { get; private set; }
        public int ExecuteAirJumpCount { get; private set; }
        public int NotifyGroundedCount { get; private set; }
        public int NotifyLeftGroundCount { get; private set; }
        public JumpType LastNotifiedJumpType { get; private set; }
        public int NotifyJumpReleasedCount { get; private set; }

        public void ExecuteNormalJump() => ExecuteNormalJumpCount++;
        public void ExecuteAirJump() => ExecuteAirJumpCount++;
        public void NotifyGrounded() => NotifyGroundedCount++;
        public void NotifyLeftGround() => NotifyLeftGroundCount++;
        public void NotifyJumpExecuted(JumpType type) => LastNotifiedJumpType = type;
        public void NotifyJumpReleased() => NotifyJumpReleasedCount++;

        public void RestoreAirJump() { }

        public void GrantAirJumps(int count) { }

        public void Tick(float deltaTime, float currentTime) { }
    }
}
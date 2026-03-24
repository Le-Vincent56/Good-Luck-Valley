using GoodLuckValley.Gameplay.Player.Interfaces;

namespace GoodLuckValley.Tests.EditMode.Gameplay.Player.Mocks
{
    public class MockBounceHandler : IBounceHandler
    {
        public bool IsBouncing { get; set; }
        public bool BouncePrepped { get; set; }
        public bool CanDetectGround { get; set; } = true;

        public int PrepareBounceCount { get; private set; }
        public int ExecuteBounceCount { get; private set; }
        public int ResetBounceCount { get; private set; }

        public void PrepareBounce(float yContactValue) => PrepareBounceCount++;
        public void ExecuteBounce() => ExecuteBounceCount++;
        public void ResetBounce() => ResetBounceCount++;

        public void Tick(float deltaTime) { }
    }
}
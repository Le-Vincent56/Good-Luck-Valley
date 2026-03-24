using GoodLuckValley.Gameplay.Player.Interfaces;

namespace GoodLuckValley.Tests.EditMode.Gameplay.Player.Mocks
{
    public class MockCrawlHandler : ICrawlHandler
    {
        public bool IsCrawling { get; set; }
        public bool CanStand { get; set; } = true;
        public float SpeedModifier { get; set; } = 1f;

        public void StartCrawl(float currentTime) => IsCrawling = true;
        public void EndCrawl() => IsCrawling = false;
        public void UpdateCeilingState(bool ceilingBlocked) => CanStand = !ceilingBlocked;
        public void Tick(float deltaTime, float currentTime) { }
    }
}
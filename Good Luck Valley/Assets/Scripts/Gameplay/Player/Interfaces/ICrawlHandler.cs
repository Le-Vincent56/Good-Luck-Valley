namespace GoodLuckValley.Gameplay.Player.Interfaces
{
    /// <summary>
    /// Represents the interface for handling player crawling behavior.
    /// Provides methods and properties for controlling and monitoring
    /// the crawl state, including starting, ending, and updating the crawl logic.
    /// </summary>
    public interface ICrawlHandler
    {
        bool IsCrawling { get; }
        bool CanStand { get; }
        float SpeedModifier { get; }

        void StartCrawl(float currentTime);
        void EndCrawl();
        void UpdateCeilingState(bool ceilingBlocked);
        void Tick(float deltaTime, float currentTime);
    }
}
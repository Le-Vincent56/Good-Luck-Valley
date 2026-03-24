namespace GoodLuckValley.Gameplay.Player.Interfaces
{
    /// <summary>
    /// Defines methods and properties for handling bounce mechanics in a gameplay context.
    /// </summary>
    public interface IBounceHandler
    {
        bool IsBouncing { get; }
        bool BouncePrepped { get; }
        bool CanDetectGround { get; }

        void PrepareBounce(float yContactValue);
        void ExecuteBounce();
        void ResetBounce();
        void Tick(float deltaTime);
    }
}
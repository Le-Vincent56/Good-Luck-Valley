namespace GoodLuckValley.Gameplay.Player.Interfaces
{
    /// <summary>
    /// Represents the interface for controlling player's ability mechanics.
    /// </summary>
    public interface IPlayerAbilityControl
    {
        void RestoreAirJump();
        void GrantAirJumps(int count);
        void PrepareBounce(float yContactValue);
    }
}
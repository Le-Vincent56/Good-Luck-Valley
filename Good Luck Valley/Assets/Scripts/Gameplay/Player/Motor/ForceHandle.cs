namespace GoodLuckValley.Gameplay.Player.Motor
{
    /// <summary>
    /// Represents a unique handle associated with a force applied in gameplay mechanics.
    /// </summary>
    public readonly struct ForceHandle
    {
        public readonly int ID;

        public ForceHandle(int id) => ID = id;
    }
}
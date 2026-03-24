namespace GoodLuckValley.Gameplay.Player.Motor
{
    /// <summary>
    /// Represents a handle for managing or referencing modifiers within the gameplay system.
    /// </summary>
    public readonly struct ModifierHandle
    {
        public readonly int ID;

        public ModifierHandle(int id) => ID = id;
    }
}
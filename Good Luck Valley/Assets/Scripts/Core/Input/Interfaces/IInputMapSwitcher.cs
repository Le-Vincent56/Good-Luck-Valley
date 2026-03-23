namespace GoodLuckValley.Core.Input.Interfaces
{
    /// <summary>
    /// Adapter-facing interface for switching action maps.
    /// Implemented by InputAdapter, consumed by InputService.
    /// Keeps InputService free of MonoBehaviour dependencies.
    /// </summary>
    public interface IInputMapSwitcher
    {
        /// <summary>
        /// Enables the player action map, allowing input actions related to player controls to be active.
        /// </summary>
        void EnablePlayerMap();

        /// <summary>
        /// Enables the UI action map, allowing input actions related to UI navigation and interaction to be active.
        /// </summary>
        void EnableUIMap();
    }
}
using UnityEngine;

namespace GoodLuckValley.Core.SceneManagement.Interfaces
{
    /// <summary>
    /// Pluggable visual transition effect. Drives a screen-space overlay from
    /// transparent (0) to opaque (1) and back. Concrete implementations
    /// (e.g., fade, wipe, dissolve) live in the World layer.
    /// </summary>
    public interface ITransitionEffect
    {
        /// <summary>
        /// Animates the transition from transparent to fully opaque.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation of the transition effect.</returns>
        Awaitable CoverAsync();

        /// <summary>
        /// Animates the transition from fully opaque back to transparent.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation of the transition effect.</returns>
        Awaitable RevealAsync();

        /// <summary>
        /// Immediately resets the effect to its idle (transparent) state.
        /// </summary>
        void Reset();
    }
}
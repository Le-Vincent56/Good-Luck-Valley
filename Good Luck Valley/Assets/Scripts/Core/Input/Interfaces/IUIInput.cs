using UnityEngine;

namespace GoodLuckValley.Core.Input.Interfaces
{
    /// <summary>
    /// Represents an input interface for UI interactions.
    /// </summary>
    public interface IUIInput
    {
        /// <summary>
        /// Gets the navigation input vector representing directional movement
        /// for UI interactions.
        /// </summary>
        Vector2 Navigate { get; }

        /// <summary>
        /// Determines whether the submit action has been triggered by the user
        /// in the UI input system.
        /// </summary>
        bool SubmitPressed { get; }

        /// <summary>
        /// Indicates whether the cancel action input is currently active
        /// during UI interactions.
        /// </summary>
        bool CancelPressed { get; }
    }
}
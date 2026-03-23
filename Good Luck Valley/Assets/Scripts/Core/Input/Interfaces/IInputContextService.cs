using System;
using GoodLuckValley.Core.Input.Enums;

namespace GoodLuckValley.Core.Input.Interfaces
{
    /// <summary>
    /// Provides functionality to manage and observe the current input context
    /// used within the application.
    /// </summary>
    public interface IInputContextService
    {
        /// <summary>
        /// Gets the current input context being used in the application, which dictates
        /// how input events or actions are processed.
        /// </summary>
        InputContext CurrentContext { get; }

        /// <summary>
        /// Sets the current input context for the application.
        /// </summary>
        /// <param name="context">
        /// The input context to be applied.
        /// Valid values include <see cref="InputContext.Player"/> and <see cref="InputContext.UI"/>.
        /// </param>
        void SetContext(InputContext context);

        /// <summary>
        /// Event that is triggered whenever the input context changes within the application.
        /// Listeners can subscribe to this event to respond to changes in the current input context, such as switching between
        /// Player and UI contexts.
        /// </summary>
        event Action<InputContext> OnContextChanged;
    }
}
using GoodLuckValley.World.Interactables;
using System;
using System.Collections.Generic;

namespace GoodLuckValley.Patterns.Commands
{
    public class InteractableCommand : ICommand<IInteractable>
    {
        List<IInteractable> interactables;
        Action<IInteractable> action = delegate { };

        private InteractableCommand() { }

        /// <summary>
        /// Execute the Interacble Command
        /// </summary>
        public void Execute()
        {
            // Loop through each interactable
            foreach(IInteractable interactable in interactables)
            {
                // Invoke the action on the interatable
                action.Invoke(interactable);
            }
        }

        public class Builder
        {
            readonly InteractableCommand command = new InteractableCommand();

            public Builder(List<IInteractable> interactables = default)
            {
                // If no interactbales were given, create a new list
                command.interactables = interactables ?? new List<IInteractable>();
            }

            /// <summary>
            /// Set the Interactable Command's action
            /// </summary>
            /// <param name="action">The Action to give the Interatable Command</param>
            /// <returns>The Builder of which was used to set the Command Action</returns>
            public Builder WithAction(Action<IInteractable> action)
            {
                // Set the action and return the builder
                command.action = action;
                return this;
            }

            /// <summary>
            /// Build the Interactable Command
            /// </summary>
            /// <returns>The built Interactable Command</returns>
            public InteractableCommand Build() => command;
        }
    }
}
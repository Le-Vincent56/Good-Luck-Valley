using GoodLuckValley.World.Decomposables;
using System.Collections.Generic;
using System;

namespace GoodLuckValley.Patterns.Commands
{
    public class DecomposeCommand : ICommand<IDecomposable>
    {
        List<IDecomposable> decomposables;
        Action<IDecomposable> action = delegate { };

        private DecomposeCommand() { }

        /// <summary>
        /// Execute the Decompose Command
        /// </summary>
        public void Execute()
        {
            // Loop through each decomposable
            foreach (IDecomposable decomposable in decomposables)
            {
                // Invoke the action on the decomposable
                action.Invoke(decomposable);
            }
        }

        public class Builder
        {
            readonly DecomposeCommand command = new DecomposeCommand();

            public Builder(List<IDecomposable> decomposables = default)
            {
                // If no interactbales were given, create a new list
                command.decomposables = decomposables ?? new List<IDecomposable>();
            }

            /// <summary>
            /// Set the Decompose Command's action
            /// </summary>
            /// <param name="action">The Action to give the Decompose Command</param>
            /// <returns>The Builder of which was used to set the Command Action</returns>
            public Builder WithAction(Action<IDecomposable> action)
            {
                // Set the action and return the builder
                command.action = action;
                return this;
            }

            /// <summary>
            /// Build the Decompose Command
            /// </summary>
            /// <returns>The built Decompose Command</returns>
            public DecomposeCommand Build() => command;
        }
    }
}
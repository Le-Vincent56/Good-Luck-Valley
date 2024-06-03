using GoodLuckValley.Patterns.Commands;

namespace GoodLuckValley.World.Interactables
{
    public interface IInteractable
    {
        /// <summary>
        /// The interaction function for the Interactable
        /// </summary>
        public void Interact();

        /// <summary>
        /// Queue a command for the Interactable
        /// </summary>
        /// <param name="command"></param>
        public void QueueCommand(ICommand<IInteractable> command);

        /// <summary>
        /// Execute the Interaction
        /// </summary>
        public void ExecuteCommand();
    }
}
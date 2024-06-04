namespace GoodLuckValley.World.Interactables
{
    public interface IInteractable
    {
        /// <summary>
        /// The interaction function for the Interactable
        /// </summary>
        public void Interact();

        /// <summary>
        /// Execute the Interaction
        /// </summary>
        public void ExecuteCommand();
    }
}
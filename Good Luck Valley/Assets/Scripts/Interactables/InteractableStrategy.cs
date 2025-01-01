using GoodLuckValley.Player.Movement;

namespace GoodLuckValley.Interactables
{
    public abstract class InteractableStrategy
    {
        public abstract bool Interact(InteractableHandler handler);
    }
}

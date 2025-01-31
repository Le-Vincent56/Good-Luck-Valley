using GoodLuckValley.Architecture.EventBus;
using GoodLuckValley.Player.Mushroom;
using GoodLuckValley.UI.Events;

namespace GoodLuckValley.Interactables.Mushroom
{
    public class MushroomPickupStrategy : InteractableStrategy
    {
        private MushroomPickup parent;

        public MushroomPickupStrategy(MushroomPickup parent)
        {
            // Set the parent Interactable
            this.parent = parent;
        }

        public override bool Interact(InteractableHandler handler)
        {
            // Unlock the Mushroom
            handler.GetComponentInChildren<MushroomSpawner>().UnlockMushroom();

            EventBus<FadeGraphic>.Raise(new FadeGraphic()
            {
                ID = parent.Hash,
                FadeIn = true
            });

            return true;
        }
    }
}

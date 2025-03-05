using GoodLuckValley.Events;
using GoodLuckValley.Events.UI;
using GoodLuckValley.Player.Mushroom;

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

            EventBus<FadeTutorialCanvasGroup>.Raise(new FadeTutorialCanvasGroup()
            {
                ID = parent.Hash,
                FadeIn = true
            });

            return true;
        }
    }
}

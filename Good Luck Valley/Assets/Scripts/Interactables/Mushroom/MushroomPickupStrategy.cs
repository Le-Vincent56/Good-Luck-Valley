using GoodLuckValley.Events;
using GoodLuckValley.Events.Cinematics;
using GoodLuckValley.Events.UI;
using GoodLuckValley.Player.Mushroom;
using UnityEngine;
using UnityEngine.Playables;

namespace GoodLuckValley.Interactables.Mushroom
{
    public class MushroomPickupStrategy : InteractableStrategy
    {
        private readonly MushroomPickup parent;
        private readonly float fadeDuration;
        private readonly PlayableDirector director;

        public MushroomPickupStrategy(MushroomPickup parent, float fadeDuration, PlayableDirector director)
        {
            // Set the parent Interactable
            this.parent = parent;
            this.fadeDuration = fadeDuration;
            this.director = director;
        }

        public override bool Interact(InteractableHandler handler)
        {
            // Set the parent to be inactive
            parent.Active = false;
            
            // Unlock the Mushroom
            handler.GetComponentInChildren<MushroomSpawner>().UnlockMushroom();
            
            EventBus<FadeInteractableCanvasGroup>.Raise(new FadeInteractableCanvasGroup()
            {
                ID = parent.Hash,
                Value = 0,
                Duration = fadeDuration,
            });

            // Start the cinematic by deactivating the player
            EventBus<StartCinematic>.Raise(new StartCinematic());
            
            // Play the cutscene
            director.Play();

            return true;
        }
    }
}

using GoodLuckValley.Architecture.Optionals;
using GoodLuckValley.Events;
using GoodLuckValley.Events.Fireflies;
using UnityEngine;

namespace GoodLuckValley.Interactables.Fireflies
{
    public class Lantern : GateInteractable
    {
        [Header("Lantern")]
        [SerializeField] private int channel;

        protected override void Awake()
        {
            // Call the parent Awake()
            base.Awake();

            // Set the strategy
            strategy = new LanternStrategy(this);
        }

        public override void Interact()
        {
            // Exit case - the Handler has no value
            if (!handler.HasValue) return;

            // Exit case - the Interactable cannot be interaacted with
            if (!canInteract) return;

            // Exit case - the Interactable Strategy fails
            if (!strategy.Interact(handler.Value)) return;

            // Remove the Interactable from the Handler
            handler.Match(
                onValue: handler =>
                {
                    handler.SetInteractable(Optional<Interactable>.NoValue);
                    return 0;
                },
                onNoValue: () => { return 0; }
            );

            // Set un-interactable
            canInteract = false;

            // Fade out the sprites and deactivate
            FadeFeedback(0f, fadeDuration);
        }

        /// <summary>
        /// Activate the effects of the Lantern
        /// </summary>
        public void Activate()
        {
            // Raise the Activate Lantern event
            EventBus<ActivateLantern>.Raise(new ActivateLantern()
            {
                Channel = channel
            });
        }

        /// <summary>
        /// Check if the Player has Fireflies for the Lantern
        /// </summary>
        protected override bool CheckForKey(InteractableHandler handler)
        {
            // Set to false by default
            bool hasKey = false;

            // Check if the Firefly Handler has a fruit
            handler.FireflyHandler.GetFireflies().Match(
                onValue: fireflies =>
                {
                    // If the player has Fireflies, they have the key
                    hasKey = true;

                    return 0;
                },
                onNoValue: () =>
                {
                    // The Firefly Handler does not have a Fruit, so do nothing
                    return 0;
                }
            );

            return hasKey;
        }
    }
}

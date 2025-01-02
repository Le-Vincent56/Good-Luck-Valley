using GoodLuckValley.Architecture.EventBus;
using GoodLuckValley.Architecture.Optionals;
using GoodLuckValley.Interactables.Fireflies.Events;
using Unity.VisualScripting;
using UnityEngine;

namespace GoodLuckValley.Interactables.Fireflies
{
    public class Lantern : Interactable
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
    }
}

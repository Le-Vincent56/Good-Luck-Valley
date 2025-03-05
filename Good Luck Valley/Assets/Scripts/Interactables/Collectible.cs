using GoodLuckValley.Architecture.Optionals;
using GoodLuckValley.Events;
using GoodLuckValley.Events.UI;
using GoodLuckValley.Persistence;
using UnityEngine;

namespace GoodLuckValley.Interactables
{
    public class Collectible : Interactable, IBind<CollectibleSaveData>
    {
        [Header("Collectible")]
        [SerializeField] protected CollectibleSaveData data;
        [SerializeField] protected bool collected;

        [field: SerializeField] public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid();

        /// <summary>
        /// Bind the Collectible data
        /// </summary>
        public void Bind(CollectibleSaveData data)
        {
            this.data = data;
            this.data.ID = ID;

            // Set collected
            collected = data.Collected;

            // Check if the Collectible has been collected
            if(collected)
            {
                // Trigger any extra collection logic
                Collect();
            }

            // Check if the game object should be active
            gameObject.SetActive(!collected);
        }

        /// <summary>
        /// Interact with the Collectible
        /// </summary>
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
            FadeInteractable(0f, fadeDuration);

            // Fade in the interactable UI
            EventBus<FadeInteractableCanvasGroup>.Raise(new FadeInteractableCanvasGroup()
            {
                ID = id,
                Value = 0f,
                Duration = fadeDuration
            });

            // Set collected
            collected = true;
            data.Collected = collected;

            // Check if any other collection logic is needed
            Collect();
        }

        protected virtual void Collect() { }
    }
}

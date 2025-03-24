using GoodLuckValley.Architecture.Optionals;
using GoodLuckValley.Events.UI;
using GoodLuckValley.Events;
using UnityEngine;

namespace GoodLuckValley.Interactables
{
    public abstract class GateInteractable : Interactable
    {
        protected bool hasKey;
        [SerializeField] private float noKeyOpacity = 0.5f;

        protected override void Awake()
        {
            // Call the parent Awake()
            base.Awake();

            // Set variables
            hasKey = false;
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            // Exit case - the Interactable cannot be interacted with
            if (!canInteract) return;

            // Exit case - there's no Interactable Handler on the colliding object
            if (!other.TryGetComponent(out InteractableHandler handler)) return;

            // Set the Interactable Handler
            this.handler = handler;

            // Set this Interactable to be handled
            handler.SetInteractable(this);

            // Check for the key
            hasKey = CheckForKey(handler);

            // Check if the player has the key
            if (hasKey) 
            {
                // Fade in the interactable UI
                EventBus<FadeInteractableCanvasGroup>.Raise(new FadeInteractableCanvasGroup()
                {
                    ID = id,
                    Value = 1f,
                    Duration = fadeDuration
                });
            }
            else
            {
                // If not, fade in the interactable UI to the no-key opacity
                EventBus<FadeInteractableCanvasGroup>.Raise(new FadeInteractableCanvasGroup()
                {
                    ID = id,
                    Value = noKeyOpacity,
                    Duration = fadeDuration
                });
            }

            // Set triggered
            triggered = true;
        }

        protected override void OnTriggerStay2D(Collider2D other)
        {
            // Exit case - the trigger has already been activated
            if (triggered) return;

            // Exit case - the Interactable cannot be interacted with
            if (!canInteract) return;

            // Exit case - there's no Interactable Handler on the colliding object
            if (!other.TryGetComponent(out InteractableHandler handler)) return;

            // Set the Interactable Handler
            this.handler = handler;

            // Set this Interactable to be handled
            handler.SetInteractable(this);

            // Check for the key
            hasKey = CheckForKey(handler);

            // Check if the player has the key
            if (hasKey)
            {
                // Fade in the interactable UI
                EventBus<FadeInteractableCanvasGroup>.Raise(new FadeInteractableCanvasGroup()
                {
                    ID = id,
                    Value = 1f,
                    Duration = fadeDuration
                });
            }
            else
            {
                // If not, fade in the interactable UI to the no-key opacity
                EventBus<FadeInteractableCanvasGroup>.Raise(new FadeInteractableCanvasGroup()
                {
                    ID = id,
                    Value = noKeyOpacity,
                    Duration = fadeDuration
                });
            }

            // Set triggered
            triggered = true;
        }

        protected override void OnTriggerExit2D(Collider2D other)
        {
            // Exit case - the Interactable cannot be interacted with
            if (!canInteract) return;

            // Exit case - there's no Interactable Handler on the colliding object
            if (!other.TryGetComponent(out InteractableHandler handler)) return;

            // Handle the Optional Interactable Handler
            this.handler.Match(
                // If there is a value, remove the Interactable from the Handler
                onValue: handler =>
                {
                    handler.SetInteractable(Optional<Interactable>.NoValue);
                    return 0;
                },
                onNoValue: () => { return 0; }
            );

            // Nullify the Interactable Handler
            this.handler = Optional<InteractableHandler>.NoValue;

            // Fade out the interactable UI
            EventBus<FadeInteractableCanvasGroup>.Raise(new FadeInteractableCanvasGroup()
            {
                ID = id,
                Value = 0f,
                Duration = fadeDuration
            });

            // Set to not triggered
            triggered = false;
        }

        /// <summary>
        ///  Check for a Key
        /// </summary>
        protected abstract bool CheckForKey(InteractableHandler interactableHandler);
    }
}

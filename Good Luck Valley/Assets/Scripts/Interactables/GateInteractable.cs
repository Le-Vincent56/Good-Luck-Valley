using GoodLuckValley.Architecture.Optionals;
using UnityEngine;

namespace GoodLuckValley.Interactables
{
    public abstract class GateInteractable : Interactable
    {
        protected bool hasKey;
        private float noKeyOpacity = 0.5f;

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
                // If so, fade in the Feedback Sprite to full opacity
                FadeFeedback(1f, fadeDuration);
            else
                // If not, fade in the feedback Sprite to the no-key opacity
                FadeFeedback(noKeyOpacity, fadeDuration);

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
                // If so, fade in the Feedback Sprite to full opacity
                FadeFeedback(1f, fadeDuration);
            else
                // If not, fade in the feedback Sprite to the no-key opacity
                FadeFeedback(noKeyOpacity, fadeDuration);

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

            // Fade out the feedback sprite
            FadeFeedback(0f, fadeDuration);

            // Set to not triggered
            triggered = false;
        }

        /// <summary>
        ///  Check for a Key
        /// </summary>
        protected abstract bool CheckForKey(InteractableHandler interactableHandler);
    }
}

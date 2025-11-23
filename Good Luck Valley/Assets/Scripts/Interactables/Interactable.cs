using DG.Tweening;
using GoodLuckValley.Architecture.Optionals;
using GoodLuckValley.Events;
using GoodLuckValley.Events.UI;
using GoodLuckValley.World.Triggers;
using UnityEngine;

namespace GoodLuckValley.Interactables
{
    public abstract class Interactable : BaseTrigger
    {
        [Header("References")]
        protected Optional<InteractableHandler> handler = Optional<InteractableHandler>.None();
        protected InteractableStrategy strategy;
        [SerializeField] protected SpriteRenderer interactableSprite;

        [Header("Fields")]
        [SerializeField] protected int id;
        [SerializeField] protected bool triggered;
        [SerializeField] protected bool canInteract;

        [Header("Tweening Variables")]
        [SerializeField] protected float fadeDuration;
        private Tween interactableFadeTween;

        public int ID => id;

        protected override void Awake()
        {
            // Call the parent Awake()
            base.Awake();

            // Get the Interactable sprite
            interactableSprite = GetComponent<SpriteRenderer>();

            // Set variables
            canInteract = true;

            // Fade out the interactable UI
            EventBus<FadeInteractableCanvasGroup>.Raise(new FadeInteractableCanvasGroup()
            {
                ID = id,
                Value = 0f,
                Duration = 0f
            });
        }

        protected virtual void OnDestroy()
        {
            // Kill any existing Tweens
            interactableFadeTween?.Kill();
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            // Exit case - the Interactable cannot be interacted with
            if (!canInteract) return;

            // Exit case - there's no Interactable Handler on the colliding object
            if (!other.TryGetComponent(out InteractableHandler handler)) return;

            // Set the Interactable Handler
            this.handler = handler;

            // Set this Interactable to be handled
            handler.SetInteractable(this);

            // Fade in the interactable UI
            EventBus<FadeInteractableCanvasGroup>.Raise(new FadeInteractableCanvasGroup()
            {
                ID = id,
                Value = 1f,
                Duration = fadeDuration
            });

            // Set triggered
            triggered = true;
        }

        protected virtual void OnTriggerStay2D(Collider2D other)
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

            // Fade in the interactable UI
            EventBus<FadeInteractableCanvasGroup>.Raise(new FadeInteractableCanvasGroup()
            {
                ID = id,
                Value = 1f,
                Duration = fadeDuration
            });

            // Set triggered
            triggered = true;
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
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

            // Set not triggered
            triggered = false;
        }

        /// <summary>
        /// Interact with the Interactable
        /// </summary>
        public virtual void Interact()
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

            // Fade out the interactable UI
            EventBus<FadeInteractableCanvasGroup>.Raise(new FadeInteractableCanvasGroup()
            {
                ID = id,
                Value = 0f,
                Duration = fadeDuration
            });
        }

        /// <summary>
        /// Handle Fade Tweening for the Interactable sprite
        /// </summary>
        protected void FadeInteractable(float endValue, float duration, TweenCallback onComplete = null)
        {
            // Kill the Fade Tween if it exists
            interactableFadeTween?.Kill();

            // Set the Fade Tween
            interactableFadeTween = interactableSprite.DOFade(endValue, duration);

            // Exit case - there's no completion action
            if (onComplete == null) return;

            // Hook up completion actions
            interactableFadeTween.onComplete = onComplete;
        }
    }
}

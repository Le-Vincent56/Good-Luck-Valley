using DG.Tweening;
using GoodLuckValley.Architecture.Optionals;
using GoodLuckValley.World.Triggers;
using System.Linq;
using UnityEngine;

namespace GoodLuckValley.Interactables
{
    public abstract class Interactable : BaseTrigger
    {
        [Header("References")]
        protected Optional<InteractableHandler> handler = Optional<InteractableHandler>.None();
        protected InteractableStrategy strategy;
        [SerializeField] protected SpriteRenderer interactableSprite;
        [SerializeField] protected SpriteRenderer[] feedbackSprites;

        [Header("Fields")]
        [SerializeField] protected bool triggered;
        [SerializeField] private bool multipleFeedbackSprites;
        [SerializeField] protected bool canInteract;

        [Header("Tweening Variables")]
        [SerializeField] protected float fadeDuration;
        private Sequence feedbackFadeSequence;
        private Tween interactableFadeTween;

        protected virtual void Awake()
        {
            // Get the Interactable sprite
            interactableSprite = GetComponent<SpriteRenderer>();

            // Check for multiple feedback sprites
            feedbackSprites = multipleFeedbackSprites 
                ? GetComponentsInChildren<SpriteRenderer>().Skip(1).ToArray()
                : GetComponentsInChildren<SpriteRenderer>().Skip(1).Take(1).ToArray();

            // Set variables
            canInteract = true;

            // Fade out the feedback sprite
            FadeFeedback(0f, 0f);
        }

        protected virtual void OnDestroy()
        {
            // Kill any existing Tweens
            feedbackFadeSequence?.Kill();
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

            // Fade in the feedback sprite
            FadeFeedback(1f, fadeDuration);

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

            // Fade in the feedback sprite
            FadeFeedback(1f, fadeDuration);

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

            // Fade out the feedback sprite
            FadeFeedback(0f, fadeDuration);

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

            // Fade out the feedback sprite
            FadeFeedback(0f, fadeDuration);
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

        /// <summary>
        /// Handle Fade Tweening for the Feedback sprite
        /// </summary>
        protected void FadeFeedback(float endValue, float duration, TweenCallback onComplete = null)
        {
            // Kill the Fade Tween if it exists
            feedbackFadeSequence?.Kill();

            // Create a new Feedback Fade Sequence
            feedbackFadeSequence = DOTween.Sequence();

            // Iterate through each Feedback Sprite
            foreach(SpriteRenderer feedbackSprite in feedbackSprites)
            {
                // Join Fade Tweens into the Sequence
                feedbackFadeSequence.Join(feedbackSprite.DOFade(endValue, duration));
            }

            // Exit case - there's no completion action
            if (onComplete == null) return;

            // Hook up completion actions
            feedbackFadeSequence.onComplete = onComplete;
        }
    }
}

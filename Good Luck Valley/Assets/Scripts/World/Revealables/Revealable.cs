using DG.Tweening;
using GoodLuckValley.Events.Fireflies;
using GoodLuckValley.Events;
using UnityEngine;
using GoodLuckValley.Timers;

namespace GoodLuckValley.World.Revealables
{
    public abstract class Revealable : MonoBehaviour
    {
        [Header("Channel")]
        [SerializeField] protected int channel;

        [Header("Delay")]
        [SerializeField] private float delay;
        protected CountdownTimer delayTimer;

        [Header("Tweening Variables")]
        [SerializeField] protected float fadeDuration;
        protected Tween fadeTween;

        private EventBinding<ActivateLantern> onActivateLantern;

        private void OnEnable()
        {
            onActivateLantern = new EventBinding<ActivateLantern>(CheckReveal);
            EventBus<ActivateLantern>.Register(onActivateLantern);
        }

        private void OnDisable()
        {
            EventBus<ActivateLantern>.Deregister(onActivateLantern);
        }

        private void OnDestroy()
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();
            delayTimer?.Dispose();
        }

        /// <summary>
        /// Reveal the revealable
        /// </summary>
        private void CheckReveal(ActivateLantern eventData)
        {
            // Exit case - the event channel does not match the Revealable channel
            if (eventData.Channel != channel) return;

            // Check if the delay timer is set
            if(delayTimer == null)
            {
                delayTimer = new CountdownTimer(delay);
                delayTimer.OnTimerStop += () => Reveal();
            }

            // Start the delay timer
            delayTimer.Start();
        }

        /// <summary>
        /// Reveal the Revealable
        /// </summary>
        protected abstract void Reveal();
    }
}

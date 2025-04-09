using DG.Tweening;
using GoodLuckValley.Events.Fireflies;
using GoodLuckValley.Events;
using UnityEngine;
using GoodLuckValley.Timers;
using System.Collections.Generic;

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

        private List<ParticleSystem> particles;

        private EventBinding<ActivateLantern> onActivateLantern;

        protected virtual void Awake()
        {
            // Get components
            particles = new List<ParticleSystem>();
            GetComponentsInChildren(particles);
        }

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
            if (delayTimer == null)
            {
                delayTimer = new CountdownTimer(delay);
                delayTimer.OnTimerStop += () =>
                {
                    // Iterate through each particle system
                    foreach (ParticleSystem ps in particles)
                    {
                        // Play the particle system
                        ps.Play();
                    }

                    // Reveal the revealable
                    Reveal();
                };
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

using DG.Tweening;
using GoodLuckValley.Architecture.EventBus;
using GoodLuckValley.Interactables.Fireflies.Events;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GoodLuckValley
{
    public class Revealable : MonoBehaviour
    {
        private Tilemap tilemap;

        [Header("Channel")]
        [SerializeField] private int channel;

        [Header("Tweening Variables")]
        [SerializeField] private float fadeDuration;
        private Tween fadeTween;

        private EventBinding<ActivateLantern> onActivateLantern;

        private void Awake()
        {
            // Get the tilemap
            tilemap = GetComponent<Tilemap>();
        }

        private void OnEnable()
        {
            onActivateLantern = new EventBinding<ActivateLantern>(Reveal);
            EventBus<ActivateLantern>.Register(onActivateLantern);
        }

        private void OnDisable()
        {
            EventBus<ActivateLantern>.Deregister(onActivateLantern);
        }

        /// <summary>
        /// Reveal the revealable
        /// </summary>
        private void Reveal(ActivateLantern eventData)
        {
            // Exit case - the event channel does not match the Revealable channel
            if (eventData.Channel != channel) return;
        }

        private void Fade(float endValue, float fadeDuration, TweenCallback onComplete = null)
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();
        }
    }
}

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
        private TilemapCollider2D tilemapCollider;

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
            tilemapCollider = GetComponent<TilemapCollider2D>();
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

            // Enable the tilemap
            tilemapCollider.enabled = true;

            // Fade in the Revealable
            Fade(1f, fadeDuration);
        }

        /// <summary>
        /// Handle Tweening for the Tilemap's opacity
        /// </summary>
        private void Fade(float endValue, float fadeDuration, TweenCallback onComplete = null)
        {
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();

            // Get the current color of the Tilemap
            Color currentColor = tilemap.color;

            // Tween the alpha channel of the Tilemap's color
            fadeTween = DOTween.To(
                () => tilemap.color,
                x => tilemap.color = x,
                new Color(currentColor.r, currentColor.g, currentColor.b, endValue),
                fadeDuration
            );

            // Exit case - there's no completion action
            if (onComplete == null) return;

            // Hook up completion actions
            fadeTween.onComplete += onComplete;
        }
    }
}

using AK.Wwise;
using DG.Tweening;
using GoodLuckValley.Timers;
using UnityEngine;

namespace GoodLuckValley.Potentiates
{
    public enum PotentiateType
    {
        TimeWarp,
    }

    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class Potentiate : MonoBehaviour
    {
        private SpriteRenderer sprite;
        [SerializeField] private PotentiateType type;
        [SerializeField] private float potentiateDuration;
        private PotentiateStrategy strategy;
        private bool canPotentiate;
        private CountdownTimer useBufferTimer;
        [SerializeField] private float useBufferTime;
        [SerializeField] private bool buffering;

        [Header("Wwise Events")]
        [SerializeField] private AK.Wwise.Event startSound;
        [SerializeField] private AK.Wwise.Event stopSound;
        [SerializeField] private RTPC primaryRTPC;
        [SerializeField] private RTPC secondaryRTPC;

        [Header("Tweening Variables")]
        [SerializeField] private float fadeDuration;
        private Tween fadeTween;

        public bool Buffering { get => buffering; }

        private void Awake()
        {
            // Get components
            sprite = GetComponent<SpriteRenderer>();

            // Set variables
            canPotentiate = true;

            // Initialize the Potentiate strategy
            switch (type)
            {
                case PotentiateType.TimeWarp:
                    strategy = new TimeWarpStrategy(this, potentiateDuration);
                    break;
            }

            // Set up the buffer timer
            useBufferTimer = new CountdownTimer(useBufferTime);
            useBufferTimer.OnTimerStart += () => buffering = true;
            useBufferTimer.OnTimerStop += () => buffering = false;
        }

        private void OnDestroy()
        {
            // Dispose of the timer
            useBufferTimer.Dispose();
            
            // Kill the Fade Tween if it exists
            fadeTween?.Kill();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Exit case - the Potentiate cannot be used
            if (!canPotentiate) return;

            // Exit case - there is no PlayerController on the collision object
            if (!collision.TryGetComponent(out PotentiateHandler handler)) return;

            // Exit case - the Potentiate strategy failed
            if (!strategy.Potentiate(handler.Controller, handler)) return;

            // Set this as the last Potentiate
            handler.SetLastPotentiate(this);

            // Set to not Potentiate
            canPotentiate = false;

            // Start buffering the Potentiate
            useBufferTimer.Start();

            // Fade out the Potentiate
            Fade(0f);
        }

        /// <summary>
        /// Handle fade tweening for the Potentiate sprite
        /// </summary>
        public void Fade(float endValue, TweenCallback onComplete = null)
        {
            // Kill the Fade tween
            fadeTween?.Kill();

            // Set the Fade tween
            fadeTween = sprite.DOFade(endValue, fadeDuration);

            // Exit case - there's no completion action
            if (onComplete == null) return;

            // Hook up completion action
            fadeTween.onComplete += onComplete;
        }

        /// <summary>
        /// Deplete the strategy
        /// </summary>
        public void Deplete() => strategy.Deplete();

        /// <summary>
        /// Allow the Potentiate to be used
        /// </summary>
        public void AllowPotentiation() => canPotentiate = true;

        /// <summary>
        /// Play the Potentiate SFX
        /// </summary>
        public void PlaySFX() => startSound.Post(gameObject);

        /// <summary>
        /// Stop any Potentiate SFX
        /// </summary>
        public void StopSFX() => stopSound.Post(gameObject);

        /// <summary>
        /// Set the value of the Primary RTPC
        /// </summary>
        public void SetPrimaryRTPC(float value) => primaryRTPC.SetGlobalValue(value);

        /// <summary>
        /// Set the value of the Secondary RTPC
        /// </summary>
        /// <param name="value"></param>
        public void SetSecondaryRTPC(float value) => secondaryRTPC.SetGlobalValue(value);
    }
}

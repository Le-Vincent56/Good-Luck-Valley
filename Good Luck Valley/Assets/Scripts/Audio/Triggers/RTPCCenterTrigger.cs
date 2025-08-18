using System;
using System.Threading;
using System.Threading.Tasks;
using AK.Wwise;
using GoodLuckValley.World.Triggers;
using UnityEngine;

namespace GoodLuckValley.Audio.Triggers
{
    public class RTPCCenterTrigger : CenterDistanceBasedTrigger
    {
        [Header("Wwise")]
        [SerializeField] private RTPC musicMuffleRTPC;
        [SerializeField] private float rtpcScale = 100f;

        [Header("Disable Behavior")]
        [SerializeField] private bool disableColliderOnShutOff = true;
        [SerializeField] private bool disableComponentOnShutOff = true;
        [SerializeField] private bool disableGameObjectOnShutOff = false;

        private Collider2D _collider2D;

        private CancellationTokenSource _fadeCts;
        private Task _fadeTask;

        protected override void Awake()
        {
            base.Awake();
            _collider2D = GetComponent<Collider2D>();
        }
        
        private void OnDisable()
        {
            // If disabled externally, cancel any fade & clear effect
            if (_fadeCts != null && !_fadeCts.IsCancellationRequested)
                _fadeCts.Cancel();

            ApplyEffects(0f);
        }

        private void OnDestroy()
        {
            _fadeCts?.Cancel();
            _fadeCts?.Dispose();
        }

        protected override void ApplyEffects(float intensity)
        {
            // Calcualte the value of the effect
            float value = Mathf.Clamp01(intensity) * rtpcScale;

            // Global is appropriate since you’re affecting MASTER_MUSIC
            musicMuffleRTPC?.SetGlobalValue(value);
        }

        /// <summary>
        /// Public entry point to fade RTPC to zero, then optionally disable the trigger.
        /// </summary>
        public void ShutOffAndDisable(float fadeSeconds = 0.8f)
        {
            // Fire-and-forget; hold a reference for overlap protection.
            _ = ShutOffAndDisableAsync(fadeSeconds);
        }

        /// <summary>
        /// Async fade with cancellation & overlap handling.
        /// </summary>
        private async Task ShutOffAndDisableAsync(float fadeSeconds = 0.8f, CancellationToken externalToken = default)
        {
            // Cancel any in-flight fade
            _fadeCts?.Cancel();
            _fadeCts?.Dispose();

            _fadeCts = CancellationTokenSource.CreateLinkedTokenSource(externalToken);
            var token = _fadeCts.Token;

            // Capture starting intensity from the base class
            float startIntensity = Mathf.Clamp01(currentIntensity);
            float duration = Mathf.Max(0f, fadeSeconds);

            try
            {
                float elapsed = 0f;
                while (elapsed < duration)
                {
                    token.ThrowIfCancellationRequested();

                    // Ease-out curve for a nice tail
                    float t = elapsed / duration;
                    float eased = 1f - Mathf.Pow(1f - t, 2f); // quadratic ease-out
                    float intensity = Mathf.Lerp(startIntensity, 0f, eased);

                    ApplyEffects(intensity);

                    // Wait until next frame on main thread
                    await Task.Yield();
                    elapsed += Time.deltaTime;
                }

                ApplyEffects(0f);
            }
            catch (OperationCanceledException)
            {
                // If canceled, ensure we don’t leave it mid-fade unintentionally
                // (no-op or set to 0 depending on your preference)
                ApplyEffects(0f);
                throw;
            }
            finally
            {
                // Disable per options once fade completes (if not canceled)
                if (!token.IsCancellationRequested)
                {
                    if (disableColliderOnShutOff && _collider2D != null) _collider2D.enabled = false;
                    if (disableComponentOnShutOff) enabled = false;
                    if (disableGameObjectOnShutOff) gameObject.SetActive(false);
                }
            }
        }
    }
}

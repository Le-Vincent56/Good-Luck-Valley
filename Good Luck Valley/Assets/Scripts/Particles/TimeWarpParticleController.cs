using DG.Tweening;
using GoodLuckValley.Timers;
using UnityEngine;

namespace GoodLuckValley
{
    public class TimeWarpParticleController : MonoBehaviour
    {
        private ParticleSystem ambientBurst;
        private ParticleSystem collectionBurst;
        private FrequencyTimer particleTimer;
        private CountdownTimer rotateTimer;

        [Header("Fields")]
        [SerializeField] private float particleInterval;

        [Header("Tweening Variables")]
        [SerializeField] private float rotateAmount;
        [SerializeField] private float percentageOffset;
        private float rotateTimeStart;
        private float rotateDuration;
        private Tween rotateTween;

        private void Awake()
        {
            // Get and set particle systems
            ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
            ambientBurst = particleSystems[0];

            // Set the rotation duration
            float ambientDuration = ambientBurst.main.duration;
            rotateTimeStart = percentageOffset * ambientDuration;
            rotateDuration = ambientDuration - rotateTimeStart;

            // Create the rotate timer
            rotateTimer = new CountdownTimer(rotateTimeStart);
            rotateTimer.OnTimerStop += () => Rotate(rotateAmount, rotateDuration);

            // Create the particle timer
            particleTimer = new FrequencyTimer(particleInterval);
            particleTimer.OnTick += () =>
            {
                // Play the particles
                ambientBurst.Play();

                // Start the rotation timer
                rotateTimer.Start();
            };

            // Play the ambient particles
            Play();
        }

        private void OnDestroy()
        {
            // Dispose of the particle timer
            particleTimer?.Dispose();
        }

        public void Play()
        {
            // Start the particle timer
            particleTimer?.Start();

            // Play the particles
            ambientBurst.Play();

            // Start the rotation timer
            rotateTimer.Start();
        }

        public void Stop() => particleTimer?.Stop();

        private void Rotate(float endValue, float duration)
        {
            // Kill the Rotate Tween if it exists
            rotateTween?.Kill();
            
            // Set the Rotate Tween
            rotateTween = transform.DORotate(new Vector3(0, 0, endValue), duration, RotateMode.LocalAxisAdd)
                .SetEase(Ease.InSine);
        }
    }
}

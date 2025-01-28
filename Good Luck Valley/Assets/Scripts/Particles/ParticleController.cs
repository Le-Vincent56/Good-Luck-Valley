using GoodLuckValley.Input;
using GoodLuckValley.Player.Movement;
using UnityEngine;

namespace GoodLuckValley.Particles
{
    public class ParticleController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameInputReader inputReader;
        [SerializeField] private ParticleSystem runningParticles;
        [SerializeField] private ParticleSystem jumpingParticles;
        [SerializeField] private ParticleSystem bouncingParticles;

        [Header("Fields")]
        [SerializeField] private bool runningParticlesActive;
        [SerializeField] private int direction;

        private void Awake()
        {
            ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
            runningParticles = particleSystems[0];
            jumpingParticles = particleSystems[1];
            bouncingParticles = particleSystems[2];
        }

        private void OnEnable()
        {
            inputReader.Move += UpdateDirection;
        }

        private void OnDisable()
        {
            inputReader.Move -= UpdateDirection;
        }

        private void UpdateDirection(Vector2 direction, bool started)
        {
            // Exit case - if the button is lifted
            if (!started) return;

            this.direction = (int)Mathf.Sign(direction.x);

            // Update the directions of the particles
            UpdateDirections();
        }

        /// <summary>
        /// Play the running particles
        /// </summary>
        public void PlayRunningParticles()
        {
            // Exit case - the running particles are already active
            if (runningParticlesActive) return;

            // Play the running particles
            runningParticles.Play();

            // Set activity
            runningParticlesActive = true;
        }

        /// <summary>
        /// Stop the running particles
        /// </summary>
        public void StopRunningParticles()
        {
            // Exit case - if the running particles are not already active
            if(!runningParticlesActive) return;

            // Stop the running particles
            runningParticles.Stop();

            // Set activity
            runningParticlesActive = false;
        }

        /// <summary>
        /// Play the jumping particles
        /// </summary>
        public void PlayJumpParticles() => jumpingParticles.Play();

        /// <summary>
        /// Update the directions of the particles
        /// </summary>
        private void UpdateDirections()
        {
            // Get the scales
            Vector3 runLocalScale = runningParticles.transform.localScale;
            Vector3 jumpLocalScale = jumpingParticles.transform.localScale;

            // Modify the scales by the direction of movement
            runLocalScale.x = Mathf.Abs(runLocalScale.x) * direction;
            jumpLocalScale.x = Mathf.Abs(jumpLocalScale.x) * direction;

            // Set the scales
            runningParticles.transform.localScale = runLocalScale;
            jumpingParticles.transform.localScale = jumpLocalScale;
        }
    }
}

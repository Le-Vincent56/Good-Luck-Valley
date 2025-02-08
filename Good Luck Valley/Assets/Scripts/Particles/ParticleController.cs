using GoodLuckValley.Input;
using GoodLuckValley.Player.Movement;
using GoodLuckValley.Timers;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace GoodLuckValley.Particles
{
    public class ParticleController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameInputReader inputReader;
        [SerializeField] private LayerDetection layerDetection;

        [Header("Running Particles")]
        [SerializeField] private ParticleSystem currentRunningParticles;
        [SerializeField] private ParticleSystem grassRunningParticles;
        [SerializeField] private ParticleSystem dirtRunningParticles;
        [SerializeField] private ParticleSystem stoneRunningParticles;

        [Header("Jumping Particles")]
        [SerializeField] private ParticleSystem jumpingParticles;

        private FrequencyTimer runParticleTimer;

        [Header("Fields")]
        [SerializeField] private float runParticleInterval;
        [SerializeField] private float initialRunScaleX;
        [SerializeField] private float initialJumpScaleX;
        [SerializeField] private float initialJumpVelocityX;
        [SerializeField] private int direction;

        private void Awake()
        {
            ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
            grassRunningParticles = particleSystems[0];
            dirtRunningParticles = particleSystems[1];
            stoneRunningParticles = particleSystems[2];
            jumpingParticles = particleSystems[3];

            // Get components
            layerDetection = GetComponentInParent<LayerDetection>();

            // Initialize the Run Particle Timer
            runParticleTimer = new FrequencyTimer(runParticleInterval);
            runParticleTimer.OnTick += PlayRunningParticles;

            // Set initial scales
            initialRunScaleX = grassRunningParticles.transform.localScale.x;
            initialJumpScaleX = jumpingParticles.transform.localScale.x;

            // Set initial velocities
            initialJumpVelocityX = jumpingParticles.velocityOverLifetime.x.constant;

            UpdateDirection(Vector2.zero, true);
        }

        private void OnEnable()
        {
            inputReader.Move += UpdateDirection;
            layerDetection.OnGroundLayerChange += SetParticleGroundLayer;
            layerDetection.OnWallTypeChange += SetParticleWallLayer;
        }

        private void OnDisable()
        {
            inputReader.Move -= UpdateDirection;
            layerDetection.OnGroundLayerChange -= SetParticleGroundLayer;
            layerDetection.OnWallTypeChange -= SetParticleWallLayer;
        }

        private void SetParticleGroundLayer(GroundType groundType)
        {
            switch(groundType)
            {
                case GroundType.Grass:
                    currentRunningParticles = grassRunningParticles;
                    break;

                case GroundType.Dirt:
                    currentRunningParticles = dirtRunningParticles;
                    break;

                case GroundType.Stone:
                    currentRunningParticles = stoneRunningParticles;
                    break;

                default:
                    break;
            }

            Debug.Log("Set Ground Layer");
        }

        private void SetParticleWallLayer(WallType wallType)
        {

        }

        private void OnDestroy()
        {
            // Dispose of the Run Particle Timer
            runParticleTimer.Dispose();
        }

        /// <summary>
        /// Update the particle directions
        /// </summary>
        private void UpdateDirection(Vector2 direction, bool started)
        {
            // Set the direction
            this.direction = (direction.x != 0) ? (int)Mathf.Sign(direction.x) : 0;

            // Update the directions of the particles
            UpdateDirections();
        }

        /// <summary>
        /// Play the running particles
        /// </summary>
        public void PlayRunningParticles()
        {
            // Play the running particle
            currentRunningParticles.Play();

            // Exit case - if the run particle timer is already running
            if (runParticleTimer.IsRunning) return;

            // Start the frequency timer
            runParticleTimer.Start();
        }

        /// <summary>
        /// Stop the running particles
        /// </summary>
        public void StopRunningParticles()
        {
            // Stop the frequency timer
            runParticleTimer.Stop();
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
            // Set the velocity of the jumping particles
            VelocityOverLifetimeModule velocityModule = jumpingParticles.velocityOverLifetime;
            float jumpVelocityX = (direction != 0) ? initialJumpVelocityX : 0;
            velocityModule.x = new MinMaxCurve(jumpVelocityX);

            // Exit case - if direction is 0
            if (direction == 0) return;

            // Get the scales
            Vector3 runLocalScale = grassRunningParticles.transform.localScale;
            Vector3 jumpLocalScale = jumpingParticles.transform.localScale;

            // Modify the scales by the direction of movement
            runLocalScale.x = initialRunScaleX * direction;
            jumpLocalScale.x = initialJumpScaleX * direction;

            // Set the scales
            grassRunningParticles.transform.localScale = runLocalScale;
            dirtRunningParticles.transform.localScale = runLocalScale;
            stoneRunningParticles.transform.localScale = runLocalScale;
            jumpingParticles.transform.localScale = jumpLocalScale;
        }
    }
}

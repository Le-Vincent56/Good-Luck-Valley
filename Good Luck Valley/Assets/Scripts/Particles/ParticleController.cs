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
        [SerializeField] private ParticleSystem currentJumpingParticles;
        [SerializeField] private ParticleSystem grassJumpingParticles;
        [SerializeField] private ParticleSystem dirtJumpingParticles;
        [SerializeField] private ParticleSystem stoneJumpingParticles;

        [Header("Dust Particles")]
        [SerializeField] private ParticleSystem currentDustParticles;
        [SerializeField] private ParticleSystem grassDustParticles;
        [SerializeField] private ParticleSystem dirtDustParticles;
        [SerializeField] private ParticleSystem stoneDustParticles;

        private FrequencyTimer runParticleTimer;

        [Header("Fields")]
        [SerializeField] private float runParticleInterval;
        [SerializeField] private float initialRunScaleX;
        [SerializeField] private float initialJumpScaleX;
        [SerializeField] private float initialGrassJumpVelocityMinX;
        [SerializeField] private float initialGrassJumpVelocityMaxX;
        [SerializeField] private float initialDirtJumpVelocityMinX;
        [SerializeField] private float initialDirtJumpVelocityMaxX;
        [SerializeField] private float initialStoneJumpVelocityMinX;
        [SerializeField] private float initialStoneJumpVelocityMaxX;
        [SerializeField] private int direction;

        private void Awake()
        {
            ParticleSystem[] particleSystems = GetComponentsInChildren<ParticleSystem>();
            grassRunningParticles = particleSystems[0];
            dirtRunningParticles = particleSystems[1];
            stoneRunningParticles = particleSystems[2];
            grassJumpingParticles = particleSystems[3];
            dirtJumpingParticles = particleSystems[4];
            stoneJumpingParticles = particleSystems[5];
            grassDustParticles = particleSystems[6];
            dirtDustParticles = particleSystems[7];
            stoneDustParticles = particleSystems[8];

            // Get components
            layerDetection = GetComponentInParent<LayerDetection>();

            // Initialize the Run Particle Timer
            runParticleTimer = new FrequencyTimer(runParticleInterval);
            runParticleTimer.OnTick += PlayRunningParticles;

            // Set initial scales
            initialRunScaleX = grassRunningParticles.transform.localScale.x;
            initialJumpScaleX = grassJumpingParticles.transform.localScale.x;

            // Set initial velocities
            initialGrassJumpVelocityMinX = grassJumpingParticles.velocityOverLifetime.x.constantMin;
            initialGrassJumpVelocityMaxX = grassJumpingParticles.velocityOverLifetime.x.constantMax;
            initialDirtJumpVelocityMinX = dirtJumpingParticles.velocityOverLifetime.x.constantMin;
            initialDirtJumpVelocityMaxX = dirtJumpingParticles.velocityOverLifetime.x.constantMax;
            initialStoneJumpVelocityMinX = stoneJumpingParticles.velocityOverLifetime.x.constantMin;
            initialStoneJumpVelocityMaxX = stoneJumpingParticles.velocityOverLifetime.x.constantMax;

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
                    currentJumpingParticles = grassJumpingParticles;
                    currentDustParticles = grassDustParticles;
                    break;

                case GroundType.Dirt:
                    currentRunningParticles = dirtRunningParticles;
                    currentJumpingParticles = dirtJumpingParticles;
                    currentDustParticles = dirtDustParticles;
                    break;

                case GroundType.Stone:
                    currentRunningParticles = stoneRunningParticles;
                    currentJumpingParticles = stoneJumpingParticles;
                    currentDustParticles = stoneDustParticles;
                    break;

                default:
                    break;
            }
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
        public void PlayJumpParticles()
        {
            // Play the current jumping particles
            currentJumpingParticles.Play();

            // Exit case - there are no dust particles for the jump
            if (currentDustParticles == null) return;

            // Play the dust particles
            currentDustParticles.Play();
        }

        /// <summary>
        /// Update the directions of the particles
        /// </summary>
        private void UpdateDirections()
        {
            // Set the velocity of the grass jumping particles
            VelocityOverLifetimeModule grassVelocityModule = grassJumpingParticles.velocityOverLifetime;
            float jumpGrassVelocityMinX = (direction != 0) ? initialGrassJumpVelocityMinX : 0;
            float jumpGrassVelocityMaxX = (direction != 0) ? initialGrassJumpVelocityMaxX : 0;
            grassVelocityModule.x = new MinMaxCurve(jumpGrassVelocityMinX, jumpGrassVelocityMaxX);

            // Set the velocity of the dirt jumping particles
            VelocityOverLifetimeModule dirtVelocityModule = dirtJumpingParticles.velocityOverLifetime;
            float jumpDirtVelocityMinX = (direction != 0) ? initialDirtJumpVelocityMinX : 0;
            float jumpDirtVelocityMaxX = (direction != 0) ? initialDirtJumpVelocityMaxX : 0;
            dirtVelocityModule.x = new MinMaxCurve(jumpDirtVelocityMinX, jumpDirtVelocityMaxX);

            // Set the velocity of the dirt jumping particles
            VelocityOverLifetimeModule stoneVelocityModule = stoneJumpingParticles.velocityOverLifetime;
            float jumpStoneVelocityMinX = (direction != 0) ? initialStoneJumpVelocityMinX : 0;
            float jumpStoneVelocityMaxX = (direction != 0) ? initialStoneJumpVelocityMaxX : 0;
            stoneVelocityModule.x = new MinMaxCurve(jumpStoneVelocityMinX, jumpStoneVelocityMaxX);

            // Exit case - if direction is 0
            if (direction == 0) return;

            // Get the scales
            Vector3 runLocalScale = grassRunningParticles.transform.localScale;
            Vector3 jumpLocalScale = grassJumpingParticles.transform.localScale;

            // Modify the scales by the direction of movement
            runLocalScale.x = initialRunScaleX * direction;
            jumpLocalScale.x = initialJumpScaleX * direction;

            // Set the scales
            grassRunningParticles.transform.localScale = runLocalScale;
            dirtRunningParticles.transform.localScale = runLocalScale;
            stoneRunningParticles.transform.localScale = runLocalScale;
            grassJumpingParticles.transform.localScale = jumpLocalScale;
            dirtJumpingParticles.transform.localScale = jumpLocalScale;
            stoneJumpingParticles.transform.localScale = jumpLocalScale;
        }
    }
}

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

        [Header("Wall Jumping Particles")]
        [SerializeField] private ParticleSystem currentWallJumpParticles;
        [SerializeField] private ParticleSystem grassWallJumpParticles;
        [SerializeField] private ParticleSystem dirtWallJumpParticles;
        [SerializeField] private ParticleSystem stoneWallJumpParticles;
        [SerializeField] private ParticleSystem bonusWallJumpParticles;

        [Header("Wall Sliding Particles")]
        [SerializeField] private ParticleSystem currentWallSlideParticles;
        [SerializeField] private ParticleSystem grassWallSlideParticles;
        [SerializeField] private ParticleSystem dirtWallSlideParticles;
        [SerializeField] private ParticleSystem stoneWallSlideParticles;
        [SerializeField] private ParticleSystem bonusWallSlideParticles;

        [Header("Dust Particles")]
        [SerializeField] private ParticleSystem currentDustParticles;
        [SerializeField] private ParticleSystem grassDustParticles;
        [SerializeField] private ParticleSystem dirtDustParticles;
        [SerializeField] private ParticleSystem stoneDustParticles;

        [Header("Wall Dust Particles")]
        [SerializeField] private ParticleSystem currentWallDustParticles;
        [SerializeField] private ParticleSystem grassWallDustParticles;
        [SerializeField] private ParticleSystem dirtWallDustParticles;
        [SerializeField] private ParticleSystem stoneWallDustParticles;

        [Header("Mushroom Float Particles")]
        [SerializeField] private ParticleSystem mushroomFloatFrontParticles;
        [SerializeField] private ParticleSystem mushroomFloatBackParticles;

        private FrequencyTimer runParticleTimer;

        [Header("Fields")]
        [SerializeField] private bool playParticles;
        [SerializeField] private float runParticleInterval;
        [SerializeField] private float initialRunScaleX;
        [SerializeField] private float initialJumpScaleX;
        [SerializeField] private float initialWallJumpX;
        [SerializeField] private float initialWallSlideX;
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

            grassWallJumpParticles = particleSystems[6];
            dirtWallJumpParticles = particleSystems[7];
            stoneWallJumpParticles = particleSystems[8];
            bonusWallJumpParticles = particleSystems[9];

            grassWallSlideParticles = particleSystems[10];
            dirtWallSlideParticles = particleSystems[11];
            stoneWallSlideParticles = particleSystems[12];
            bonusWallSlideParticles = particleSystems[13];

            grassDustParticles = particleSystems[14];
            dirtDustParticles = particleSystems[15];
            stoneDustParticles = particleSystems[16];

            grassWallDustParticles = particleSystems[17];
            dirtWallDustParticles = particleSystems[18];
            stoneWallDustParticles = particleSystems[19];

            mushroomFloatFrontParticles = particleSystems[20];
            mushroomFloatBackParticles = particleSystems[21];

            // Get components
            layerDetection = GetComponentInParent<LayerDetection>();

            // Initialize the Run Particle Timer
            runParticleTimer = new FrequencyTimer(runParticleInterval);
            runParticleTimer.OnTick += PlayRunningParticles;

            // Set initial scales
            initialRunScaleX = grassRunningParticles.transform.localScale.x;
            initialJumpScaleX = grassJumpingParticles.transform.localScale.x;
            initialWallJumpX = grassWallJumpParticles.transform.localScale.x;
            initialWallSlideX = grassWallSlideParticles.transform.localScale.x;

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
            layerDetection.OnWallDirectionChange += UpdateWallDirection;
        }

        private void OnDisable()
        {
            inputReader.Move -= UpdateDirection;
            layerDetection.OnGroundLayerChange -= SetParticleGroundLayer;
            layerDetection.OnWallTypeChange -= SetParticleWallLayer;
            layerDetection.OnWallDirectionChange -= UpdateWallDirection;
        }

        private void OnDestroy()
        {
            // Dispose of the Run Particle Timer
            runParticleTimer.Dispose();
        }

        /// <summary>
        /// Set the particles to match the ground layer
        /// </summary>
        /// <param name="groundType"></param>
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

        /// <summary>
        /// Set the particles to match the wall layer
        /// </summary>
        private void SetParticleWallLayer(WallType wallType)
        {
            switch (wallType)
            {
                case WallType.Grass:
                    currentWallJumpParticles = grassWallJumpParticles;
                    currentWallSlideParticles = grassWallSlideParticles;
                    currentWallDustParticles = grassWallDustParticles;
                    break;

                case WallType.Dirt:
                    currentWallJumpParticles = dirtWallJumpParticles;
                    currentWallSlideParticles = dirtWallSlideParticles;
                    currentWallDustParticles = dirtWallDustParticles;
                    break;

                case WallType.Stone:
                    currentWallJumpParticles = stoneWallJumpParticles;
                    currentWallSlideParticles = stoneWallSlideParticles;
                    currentWallDustParticles = stoneWallDustParticles;
                    break;

                default:
                    break;
            }
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

        private void UpdateWallDirection(int direction)
        {
            // Get the scales
            Vector3 wallJumpLocalScale = currentWallJumpParticles.transform.localScale;
            Vector3 wallSlideLocalScale = currentWallSlideParticles.transform.localScale;

            // Modify the scales by the direction of movement
            wallJumpLocalScale.x = initialWallJumpX * direction;
            wallSlideLocalScale.x = initialWallSlideX * -direction;

            // Set the scales
            grassWallJumpParticles.transform.localScale = wallJumpLocalScale;
            dirtWallJumpParticles.transform.localScale = wallJumpLocalScale;
            stoneWallJumpParticles.transform.localScale = wallJumpLocalScale;
            bonusWallJumpParticles.transform.localScale = wallJumpLocalScale;
            grassWallSlideParticles.transform.localScale = wallSlideLocalScale;
            dirtWallSlideParticles.transform.localScale = wallSlideLocalScale;
            stoneWallSlideParticles.transform.localScale = wallSlideLocalScale;
            bonusWallSlideParticles.transform.localScale = wallSlideLocalScale;
        }

        /// <summary>
        /// Play the running particles
        /// </summary>
        public void PlayRunningParticles()
        {
            // Exit case - if not supposed to play particles
            if (!playParticles) return;

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
            // Exit case - if not supposed to play particles
            if (!playParticles) return;

            // Stop the frequency timer
            runParticleTimer.Stop();
        }

        /// <summary>
        /// Play the jumping particles
        /// </summary>
        public void PlayJumpParticles()
        {
            // Exit case - if not supposed to play particles
            if (!playParticles) return;

            // Play the current jumping particles
            currentJumpingParticles.Play();

            // Exit case - there are no dust particles for the jump
            if (currentDustParticles == null) return;

            // Play the dust particles
            currentDustParticles.Play();
        }

        /// <summary>
        /// Play the wall jump particles
        /// </summary>
        public void PlayWallJumpParticles()
        {
            // Exit case - if not supposed to play particles
            if (!playParticles) return;

            // Play the current wall jump particles
            currentWallJumpParticles.Play();
            bonusWallJumpParticles.Play();

            // Exit case - there are no dust particles for the wall jump
            if (currentWallDustParticles == null) return;

            // Play the dust particles
            currentWallDustParticles.Play();
        }

        /// <summary>
        /// Play the Wall Slide particles
        /// </summary>
        public void PlayWallSlideParticles()
        {
            // Exit case - if not supposed to play particles
            if (!playParticles) return;

            // Play the current wall sliding particles
            currentWallSlideParticles.Play();
            bonusWallSlideParticles.Play();

            // Exit case - there are no dust particles for the wall slide
            if (currentWallDustParticles == null) return;

            // Play the dust particles
            currentWallDustParticles.Play();
        }

        /// <summary>
        /// Stop the Wall Slide particles
        /// </summary>
        public void StopWallSlideParticles()
        {
            // Exit case - if not supposed to play particles
            if (!playParticles) return;

            currentWallSlideParticles.Stop();
            bonusWallSlideParticles.Stop();

            // Exit case - there are no dust particles for the wall slide
            if (currentWallDustParticles == null) return;

            // Play the dust particles
            currentWallDustParticles.Stop();
        }

        /// <summary>
        /// Play the Mushroom Float particles
        /// </summary>
        public void PlayFloatParticles()
        {
            // Exit case - if not supposed to play particles
            if (!playParticles) return;

            mushroomFloatFrontParticles.Play();
            mushroomFloatBackParticles.Play();
        }

        /// <summary>
        /// Play the Mushroom Float particles
        /// </summary>
        public void StopFloatParticles()
        {
            // Exit case - if not supposed to play particles
            if (!playParticles) return;

            mushroomFloatFrontParticles.Stop();
            mushroomFloatBackParticles.Stop();
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

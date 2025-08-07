using GoodLuckValley.Events;
using GoodLuckValley.Events.Development;
using GoodLuckValley.Input;
using GoodLuckValley.Player.Movement;
using GoodLuckValley.Timers;
using GoodLuckValley.VFX;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.ParticleSystem;

namespace GoodLuckValley.VFX.Particles
{
    public class PlayerParticleController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private LayerMask particleLayer;
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
        [SerializeField] private ParticleSystem grassWallJumpPrefab;
        [SerializeField] private ParticleSystem dirtWallJumpPrefab;
        [SerializeField] private ParticleSystem stoneWallJumpPrefab;
        [SerializeField] private ParticleSystem bonusWallJumpPrefab;
        private WallType wallType;
        private int lastWallDirection;

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

        [Header("Particle Pools")]
        [SerializeField] private List<(ParticleSystem Particles, ParticlePool Pool)> activePoolParticles;
        [SerializeField] private List<(ParticleSystem Particles, ParticlePool Pool)> particlesToRemove;
        private ParticlePool grassWallJumpPool;
        private ParticlePool dirtWallJumpPool;
        private ParticlePool stoneWallJumpPool;
        private ParticlePool bonusWallJumpPool;

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

        private EventBinding<ChangeDevelopmentTools> onChangeDevelopmentTools;

        private void Awake()
        {
            List<ParticleSystem> particleSystems = new List<ParticleSystem>();

            // Collect Particle Systems
            GetComponentsInChildren(particleSystems);

            // Set the Particle Systems
            grassRunningParticles = particleSystems[0];
            dirtRunningParticles = particleSystems[1];
            stoneRunningParticles = particleSystems[2];

            grassJumpingParticles = particleSystems[3];
            dirtJumpingParticles = particleSystems[4];
            stoneJumpingParticles = particleSystems[5];

            grassWallSlideParticles = particleSystems[6];
            dirtWallSlideParticles = particleSystems[7];
            stoneWallSlideParticles = particleSystems[8];
            bonusWallSlideParticles = particleSystems[9];

            grassDustParticles = particleSystems[10];
            dirtDustParticles = particleSystems[11];
            stoneDustParticles = particleSystems[12];

            grassWallDustParticles = particleSystems[13];
            dirtWallDustParticles = particleSystems[14];
            stoneWallDustParticles = particleSystems[15];

            mushroomFloatFrontParticles = particleSystems[16];
            mushroomFloatBackParticles = particleSystems[17];

            // Create Object Pools
            grassWallJumpPool = new ParticlePool(grassWallJumpPrefab);
            dirtWallJumpPool = new ParticlePool(dirtWallJumpPrefab);
            stoneWallJumpPool = new ParticlePool(stoneWallJumpPrefab);
            bonusWallJumpPool = new ParticlePool(bonusWallJumpPrefab);

            // Initialize the particles list
            activePoolParticles = new List<(ParticleSystem Particles, ParticlePool Pool)>();
            particlesToRemove = new List<(ParticleSystem Particles, ParticlePool Pool)>();

            // Get components
            layerDetection = GetComponentInParent<LayerDetection>();

            // Initialize the Run Particle Timer
            runParticleTimer = new FrequencyTimer(runParticleInterval);
            runParticleTimer.OnTick += PlayRunningParticles;

            // Set initial scales
            initialRunScaleX = grassRunningParticles.transform.localScale.x;
            initialJumpScaleX = grassJumpingParticles.transform.localScale.x;
            initialWallJumpX = grassWallJumpPrefab.transform.localScale.x;
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

            onChangeDevelopmentTools = new EventBinding<ChangeDevelopmentTools>(SetVisibility);
            EventBus<ChangeDevelopmentTools>.Register(onChangeDevelopmentTools);
        }

        private void OnDisable()
        {
            inputReader.Move -= UpdateDirection;
            layerDetection.OnGroundLayerChange -= SetParticleGroundLayer;
            layerDetection.OnWallTypeChange -= SetParticleWallLayer;
            layerDetection.OnWallDirectionChange -= UpdateWallDirection;

            EventBus<ChangeDevelopmentTools>.Deregister(onChangeDevelopmentTools);
        }

        private void OnDestroy()
        {
            // Dispose of the Run Particle Timer
            runParticleTimer.Dispose();
        }

        private void Update()
        {
            // Exit case - there are no active particles
            if (activePoolParticles.Count <= 0) return;

            // Clear the particles to remove List
            particlesToRemove.Clear();

            // Iterate through the active particles
            foreach ((ParticleSystem Particles, ParticlePool Pool) poolParticles in activePoolParticles)
            {
                // Skip if the Particle System is still playing
                if (poolParticles.Particles.isPlaying) continue;

                // Release the Particle System back to the pool
                poolParticles.Pool.Release(poolParticles.Particles);

                // Flag the Particle System for removal
                particlesToRemove.Add(poolParticles);
            }

            // Iterate through each Particle System to remove
            foreach((ParticleSystem Particles, ParticlePool Pool) in particlesToRemove)
            {
                // Remove the Particle System from the active particles list
                activePoolParticles.Remove((Particles, Pool));
            }
        }

        /// <summary>
        /// Get the first checked Layer of the Layer Mask
        /// </summary>
        private int GetFirstLayerFromMask(LayerMask mask)
        {
            // Get the bit mask from the LayerMask
            int bitmask = mask.value;

            // Iterate through each layer
            for (int i = 0; i < 32; i++) 
            {
                // Bitshift to find the first checked layer
                if ((bitmask & (1 << i)) != 0)
                    return i;
            }

            // Return -1 if no layers are checked
            return -1;
        }

        /// <summary>
        /// Set the particles to match the ground layer
        /// </summary>
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
                    this.wallType = wallType;
                    currentWallSlideParticles = grassWallSlideParticles;
                    currentWallDustParticles = grassWallDustParticles;
                    break;

                case WallType.Dirt:
                    this.wallType = wallType;
                    currentWallSlideParticles = dirtWallSlideParticles;
                    currentWallDustParticles = dirtWallDustParticles;
                    break;

                case WallType.Stone:
                    this.wallType = wallType;
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
            lastWallDirection = direction;
            Vector3 wallSlideLocalScale = currentWallSlideParticles.transform.localScale;

            // Modify the scales by the direction of movement
            wallSlideLocalScale.x = initialWallSlideX * -direction;

            // Set the scales
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

            // Determine the Particle Pool to retrieve from
            ParticlePool poolToRetrieveFrom = wallType switch
            {
                WallType.Grass => grassWallJumpPool,
                WallType.Dirt => dirtWallJumpPool,
                WallType.Stone => stoneWallJumpPool,
                WallType.None => null,
                _ => null,
            };

            // Exit case - there is no given pool to retrieve from
            if(poolToRetrieveFrom == null) return;

            // Create the base wall jump particles
            ParticleSystem baseWallParticles = poolToRetrieveFrom.Get();

            // Exit case - there are no wall jump particles
            if (baseWallParticles == null) return;

            // Get the bonus wall jump particles
            ParticleSystem bonusWallParticles = bonusWallJumpPool.Get();

            Transform baseTransform = baseWallParticles.transform;
            Transform bonusTransform = bonusWallParticles.transform;

            // Set this as the parent
            baseTransform.SetParent(transform);
            bonusTransform.SetParent(transform);

            // Set positions
            baseTransform.localPosition = Vector3.zero;
            bonusTransform.localPosition = Vector3.zero;

            // Set the scale
            Vector3 wallJumpLocalScale = baseTransform.localScale;
            wallJumpLocalScale.x = initialWallJumpX * -lastWallDirection;
            baseTransform.localScale = wallJumpLocalScale;
            bonusTransform.localScale = wallJumpLocalScale;

            // Play the current wall jump particles
            baseWallParticles.Play();

            // Play the bonus wall jump particles
            bonusWallParticles.Play();

            // Track the active particles
            activePoolParticles.Add((baseWallParticles, poolToRetrieveFrom));
            activePoolParticles.Add((bonusWallParticles, bonusWallJumpPool));

            // Get the first checked layer from the LayerMask
            int selectedLayer = GetFirstLayerFromMask(particleLayer);

            // Check if the layer is valid
            if (selectedLayer != -1)
            {
                // Set the layer of the particles
                baseWallParticles.gameObject.layer = selectedLayer;
                bonusWallParticles.gameObject.layer = selectedLayer;
            }

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

        /// <summary>
        /// Set whether or not to play the particles depending on visibility
        /// </summary>
        private void SetVisibility(ChangeDevelopmentTools changeDevelopmentTools)
        {
            // Set the active state
            playParticles = !changeDevelopmentTools.Invisible;
        }
    }
}

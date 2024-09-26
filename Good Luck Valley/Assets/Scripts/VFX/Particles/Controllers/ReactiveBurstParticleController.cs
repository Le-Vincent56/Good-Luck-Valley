using GoodLuckValley.Events;
using GoodLuckValley.Mushroom;
using GoodLuckValley.Player.Control;
using GoodLuckValley.VFX.ScriptableObjects;
using GoodLuckValley.World.Tiles;
using UnityEngine;
using UnityEngine.VFX;

namespace GoodLuckValley.VFX.Particles.Controllers
{
    // Enum for currently active particle
    enum ActiveParticle
    {
        GrassRunning,
        GrassJumping,
        GrassLanding,
        DirtRunning,
        DirtJumping,
        DirtLanding,
        ShroomGrow,
        ShroomBounce,
        None
    }

    public class ReactiveBurstParticleController : MonoBehaviour
    {
        #region REFERENCES
        [Header("General")]
        [SerializeField] private ParticleControllerDetectTileType tileTypeDetector;
        [SerializeField] private GameEvent onRequestHardpoints;
        private VisualEffect burstParticleVFX;
        private PlayerController playerController;

        [Header("Particle Data")]
        [SerializeField] private VFXBurstParticleData grassRunningData;
        [SerializeField] private VFXBurstParticleData grassLandData;
        [SerializeField] private VFXBurstParticleData grassJumpData;
        [SerializeField] private VFXBurstParticleData dirtRunningData;
        [SerializeField] private VFXBurstParticleData dirtLandData;
        [SerializeField] private VFXBurstParticleData dirtJumpData;
        #endregion

        #region FIELDS
        [SerializeField] private ActiveParticle activeParticle;
        private bool lastSentWasRunningData;
        private Transform runHardpoint;
        private Transform bounceHardpoint;
        private Transform landHardpoint;
        private Transform jumpHardpoint;

        [Header("Grass Running Particle Fields")]
        [SerializeField] private float minRunningParticleVelocity;
        [SerializeField] private float timeBetweenParticles;
        [SerializeField] private float runningParticlesCurrentTime;
        #endregion

        private void Awake()
        {
            activeParticle = ActiveParticle.None;
            HandleRunningParticles();
            burstParticleVFX = GetComponent<VisualEffect>();
        }

        private void Start()
        {
            // If the hardpoints were not set during awake calls, request them now
            if (runHardpoint == null)
                onRequestHardpoints.Raise(null, null);
        }

        private void Update()
        {
            // Check to make sure the necessary references are not null
            if (playerController != null && runHardpoint != null && bounceHardpoint != null)
            {
                // Check currently active particle
                switch (activeParticle)
                {
                    // No particle active
                    case ActiveParticle.None:
                        break;

                    // Grass running particle active
                    case ActiveParticle.GrassRunning:

                        // Increment running particles time using deltaTime
                        runningParticlesCurrentTime += Time.deltaTime;

                        // Ensure player is grounded,
                        //  moving above the minimum velocity,
                        //  and there has been enough time between running particles to spawn one
                        if (playerController.IsGrounded &&
                            Mathf.Abs(playerController.Velocity.x) > minRunningParticleVelocity &&
                            runningParticlesCurrentTime > timeBetweenParticles)
                        {
                            // Send VFX data
                            SendVFXData(grassRunningData);

                            // Set the VFX spawn position
                            SendVFXSpawnPosition(runHardpoint.position);

                            // Play the particle
                            burstParticleVFX.Play();
                            
                            // Reset the running time
                            runningParticlesCurrentTime = 0;
                        }
                        break;

                    // Grass landing particle active
                    case ActiveParticle.GrassLanding:

                        // Send VFX data
                        SendVFXData(grassLandData);

                        // Set the VFX spawn position
                        SendVFXSpawnPosition(landHardpoint.position);    

                        // Play the particle
                        burstParticleVFX.Play();

                        // Reset to the running particle
                        HandleRunningParticles();
                        break;

                    // Grass jumping particle active
                    case ActiveParticle.GrassJumping:

                        // Send VFX data
                        SendVFXData(grassJumpData);

                        // If player is moving above minimum velocity, use the jump hardpoint (offset in front of player to account for speed)
                        //  if not, use the land one (directly under player)
                        if (Mathf.Abs(playerController.Velocity.x) > minRunningParticleVelocity)
                            SendVFXSpawnPosition(jumpHardpoint.position);
                        else
                            SendVFXSpawnPosition(landHardpoint.position);

                        // Play the particle
                        burstParticleVFX.Play();

                        // Reset to the running particle
                        HandleRunningParticles();
                        break;

                    // Grass running particle active
                    case ActiveParticle.DirtRunning:

                        // Increment running particles time using deltaTime
                        runningParticlesCurrentTime += Time.deltaTime;

                        // Ensure player is grounded,
                        //  moving above the minimum velocity,
                        //  and there has been enough time between running particles to spawn one
                        if (playerController.IsGrounded &&
                            Mathf.Abs(playerController.Velocity.x) > minRunningParticleVelocity &&
                            runningParticlesCurrentTime > timeBetweenParticles)
                        {
                            // Send VFX data
                            SendVFXData(dirtRunningData);

                            // Set the VFX spawn position
                            SendVFXSpawnPosition(runHardpoint.position);

                            // Play the particle
                            burstParticleVFX.Play();

                            // Reset the running time
                            runningParticlesCurrentTime = 0;
                        }
                        break;

                    // Grass landing particle active
                    case ActiveParticle.DirtLanding:
                        // Send VFX data
                        SendVFXData(dirtLandData);

                        // Set the VFX spawn position
                        SendVFXSpawnPosition(landHardpoint.position);

                        // Play the particle
                        burstParticleVFX.Play();

                        // Reset to the running particle
                        HandleRunningParticles();
                        break;

                    // Grass jumping particle active
                    case ActiveParticle.DirtJumping:
                        // Send VFX data
                        SendVFXData(dirtJumpData);

                        // If player is moving above minimum velocity, use the jump hardpoint (offset in front of player to account for speed)
                        //  if not, use the land one (directly under player)
                        if (Mathf.Abs(playerController.Velocity.x) > minRunningParticleVelocity)
                            SendVFXSpawnPosition(jumpHardpoint.position);
                        else
                            SendVFXSpawnPosition(landHardpoint.position);

                        // Play the particle
                        burstParticleVFX.Play();

                        // Reset to the running particle
                        HandleRunningParticles();
                        break;
                }
            }
        }

        #region SENDING DATA TO GPU
        private void SendVFXData(VFXBurstParticleData data)
        {
            // If the current particle is a running particle and the running particle data has already been sent
            if ((activeParticle == ActiveParticle.GrassRunning || activeParticle == ActiveParticle.DirtRunning) && lastSentWasRunningData)
            {
                // If so, return
                return;
            }
            // If running data has not already been sent, it will be now, set bool to true
            else if (activeParticle == ActiveParticle.GrassRunning || activeParticle == ActiveParticle.DirtRunning)
            {
                lastSentWasRunningData = true;
            }
            // Otherwise we are setting different data, so the last sent data was not running data
            else
            {
                lastSentWasRunningData = false;
            }

            // Spawn data
            burstParticleVFX.SetFloat("SpawnCount", data.spawnCount);

            // Initialize data
            burstParticleVFX.SetFloat("Min X Velocity", data.minXVelocity);
            burstParticleVFX.SetFloat("Max X Velocity", data.maxXVelocity);
            burstParticleVFX.SetFloat("Min Y Velocity", data.minYVelocity);
            burstParticleVFX.SetFloat("Max Y Velocity", data.maxYVelocity);
            burstParticleVFX.SetFloat("Min Lifetime", data.minLifetime);
            burstParticleVFX.SetFloat("Max Lifetime", data.maxLifetime);
            burstParticleVFX.SetFloat("Min Size", data.minSize);
            burstParticleVFX.SetFloat("Max Size", data.maxSize);
            burstParticleVFX.SetFloat("Min Starting Angle", data.minStartingAngle);
            burstParticleVFX.SetFloat("Max Starting Angle", data.maxStartingAngle);    

            // Update data
            burstParticleVFX.SetFloat("Gravity", data.gravity);
            burstParticleVFX.SetFloat("Min Linear Drag Coefficient", data.minLinearDragCoefficient);
            burstParticleVFX.SetFloat("Max Linear Drag Coefficient", data.maxLinearDragCoefficient);
            burstParticleVFX.SetFloat("Turb Intensity", data.turbulenceIntensity);
            burstParticleVFX.SetFloat("Turb Drag", data.turbulenceDrag);
            burstParticleVFX.SetFloat("Turb Frequency", data.turbulenceFrequency);
            burstParticleVFX.SetFloat("Add Z Angle", data.addZAngle);
            burstParticleVFX.SetFloat("Min Add Angle Scalar", data.minAddAngleScalar);
            burstParticleVFX.SetFloat("Max Add Angle Scalar", data.maxAddAngleScalar);

            // Output data
            burstParticleVFX.SetTexture("MainTex", data.mainTex);
            burstParticleVFX.SetAnimationCurve("Mult Size / Life", data.multiplySizeOverLife);
            burstParticleVFX.SetGradient("Color / Life", data.colorOverLife);
        }

        /// <summary>
        /// Sends the spawn position information to the GPU
        /// </summary>
        /// <param name="position"> The position to spawn the particle at </param>
        private void SendVFXSpawnPosition(Vector3 position)
        {
            // Sets vector 3 data
            burstParticleVFX.SetVector3("SpawnPosition", position);
        }
        #endregion

        #region PLAYER PARTICLES HANDLERS
        /// <summary>
        /// Handles detection and setting active particle information for jump particles
        /// </summary>
        public void HandleJumpParticles()
        {
            switch(tileTypeDetector.GetTileType())
            {
                case TileType.Grass:
                    activeParticle = ActiveParticle.GrassJumping;
                    break;
                case TileType.Dirt:
                    activeParticle = ActiveParticle.DirtJumping;
                    break;
            }
        }

        /// <summary>
        /// Handles detection and setting active particle information for land particles
        /// </summary>
        public void HandleLandParticles()
        {
            // Check tile type
            switch (tileTypeDetector.GetTileType())
            {
                // Set to grass if grass tile detected
                case TileType.Grass:
                    activeParticle = ActiveParticle.GrassLanding;
                    break;

                // Set to dirt if dirt tile detected
                case TileType.Dirt:
                    activeParticle = ActiveParticle.DirtLanding;
                    break;
            }
        }

        /// <summary>
        /// Handles detection and setting active particle information for running particles
        /// </summary>
        private void HandleRunningParticles()
        {
            // Check tile type
            switch (tileTypeDetector.GetTileType())
            {
                // Set to grass if grass tile detected
                case TileType.Grass:
                    activeParticle = ActiveParticle.GrassRunning;
                    break;

                // Set to dirt if dirt tile detected
                case TileType.Dirt:
                    activeParticle = ActiveParticle.DirtRunning;
                    break;
            }
        }
        #endregion

        #region INITIALIZE REFERENCES
        /// <summary>
        /// Initializes the player controller
        /// </summary>
        /// <param name="sender"> Component that sent the data </param>
        /// <param name="data"> Data being sent </param>
        public void InitializePC(Component sender, object data)
        {
            // Return if sent data is not PC
            if (data is not PlayerController) return;

            // Initialize PC to sent data
            playerController = (PlayerController)data;
        }

        /// <summary>
        /// Initializes the hardpoints
        /// </summary>
        /// <param name="sender"> Component that sent the data </param>
        /// <param name="data"> Data being sent </param>
        public void InitializeHardpoints(Component sender, object data)
        {
            // Return if data is not hardpoints
            if (data is not PlayerHardpointsContainer.Hardpoints) return;

            // Get hardpoint object from data
            PlayerHardpointsContainer.Hardpoints hardpoints = (PlayerHardpointsContainer.Hardpoints)data;
            
            // Assign hardpoints to appropriate values in data
            runHardpoint = hardpoints.Run;
            bounceHardpoint = hardpoints.Bounce;
            jumpHardpoint = hardpoints.Jump;
            landHardpoint = hardpoints.Land;
        }
        #endregion
    }
}
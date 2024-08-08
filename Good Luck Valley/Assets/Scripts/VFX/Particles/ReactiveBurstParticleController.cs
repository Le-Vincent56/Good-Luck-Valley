using GoodLuckValley.Events;
using GoodLuckValley.Player.Control;
using GoodLuckValley.VFX.ScriptableObjects;
using GoodLuckValley.World.Tiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace GoodLuckValley.VFX.Particles
{
    enum ActiveParticle
    {
        GrassRunning,
        GrassJumping,
        GrassLanding,
        DirtRunning,
        DirtJumping,
        DirtLanding,
        None
    }

    public class ReactiveBurstParticleController : MonoBehaviour
    {
        #region REFERENCES
        [Header("General")]
        [SerializeField] private ParticleControllerDetectTileType tileTypeDetector;
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
        private bool setRunningData;

        [Header("Grass Running Particle Fields")]
        [SerializeField] private float minRunningParticleVelocity;
        [SerializeField] private float timeBetweenParticles;
        [SerializeField] private float runningParticlesCurrentTime;
        #endregion

        private void Awake()
        {
            activeParticle = ActiveParticle.None;
            burstParticleVFX = GetComponent<VisualEffect>();
        }

        private void Update()
        {
            if (playerController != null)
            {
                switch (activeParticle)
                {
                    case ActiveParticle.None:
                        Debug.Log("NO PARTICLE ACTIVE");
                        break;

                    case ActiveParticle.GrassRunning:
                        runningParticlesCurrentTime += Time.deltaTime;

                        if (playerController.IsGrounded &&
                            Mathf.Abs(playerController.Velocity.x) > minRunningParticleVelocity &&
                            runningParticlesCurrentTime > timeBetweenParticles)
                        {
                            SetVFXData(grassRunningData);
                            SetVFXSpawnPosition(playerController.transform.position);
                            burstParticleVFX.Play();
                            runningParticlesCurrentTime = 0;
                        }
                        break;

                    case ActiveParticle.GrassLanding:
                        // Play GL particle
                        SetVFXData(grassJumpData);
                        SetVFXSpawnPosition(playerController.transform.position);
                        burstParticleVFX.Play();
                        ResetToRunningParticle();
                        break;

                    case ActiveParticle.GrassJumping:
                        Debug.Log("Playing grass jump");
                        SetVFXData(grassJumpData);
                        SetVFXSpawnPosition(playerController.transform.position);
                        burstParticleVFX.Play();
                        activeParticle = ActiveParticle.None;
                        break;

                    case ActiveParticle.DirtRunning:
                        break;

                    case ActiveParticle.DirtLanding:
                        break;

                    case ActiveParticle.DirtJumping:
                        break;
                }
            }
        }

        private void SetVFXData(VFXBurstParticleData data)
        {
            if ((activeParticle == ActiveParticle.GrassRunning || activeParticle == ActiveParticle.DirtRunning) && setRunningData)
            {
                return;
            }
            else if ((activeParticle == ActiveParticle.GrassRunning || activeParticle == ActiveParticle.DirtRunning))
            {
                setRunningData = true;
            }
            else
            {
                setRunningData = false;
            }

            // Spawn
            burstParticleVFX.SetFloat("SpawnCount", data.spawnCount);

            // Initialize
            burstParticleVFX.SetFloat("Min X Velocity", data.minXVelocity);
            burstParticleVFX.SetFloat("Max X Velocity", data.maxXVelocity);
            burstParticleVFX.SetFloat("Min Y Velocity", data.minYVelocity);
            burstParticleVFX.SetFloat("Max Y Velocity", data.maxYVelocity);
            burstParticleVFX.SetFloat("Min Lifetime", data.minLifetime);
            burstParticleVFX.SetFloat("Max Lifetime", data.maxLifetime);
            burstParticleVFX.SetFloat("Min Size", data.minSize);
            burstParticleVFX.SetFloat("Max Size", data.maxSize);

            // Update
            burstParticleVFX.SetFloat("Gravity", data.gravity);
            burstParticleVFX.SetFloat("Linear Drag Coefficient", data.linearDragCoefficient);
            burstParticleVFX.SetFloat("Turb Intensity", data.turbulenceIntensity);
            burstParticleVFX.SetFloat("Turb Drag", data.turbulenceDrag);
            burstParticleVFX.SetFloat("Turb Frequency", data.turbulenceFrequency);
            burstParticleVFX.SetFloat("Add Z Angle", data.addZAngle);
            burstParticleVFX.SetFloat("Min Add Angle Scalar", data.minAddAngleScalar);
            burstParticleVFX.SetFloat("Max Add Angle Scalar", data.maxAddAngleScalar);

            // Output
            burstParticleVFX.SetTexture("MainTex", data.mainTex);
            burstParticleVFX.SetAnimationCurve("Mult Size / Life", data.multiplySizeOverLife);
            burstParticleVFX.SetGradient("Color / Life", data.colorOverLife);
        }

        public void HandleJumpParticles()
        {
            Debug.Log("Jump Particles!");   
            switch (tileTypeDetector.GetTileType())
            {
                case TileType.None: break;
                case TileType.Grass:
                    activeParticle = ActiveParticle.GrassJumping;
                    Debug.Log("grass jump");

                    break;
                case TileType.Dirt:
                    activeParticle = ActiveParticle.DirtJumping;
                    Debug.Log("dirt jump");
                    break;
            }
        }

        public void HandleLandParticles()
        {
            Debug.Log("Land Particles!");
            switch (tileTypeDetector.GetTileType())
            {
                case TileType.None: break;
                case TileType.Grass:
                    activeParticle = ActiveParticle.GrassLanding;
                    Debug.Log("grass land");

                    break;
                case TileType.Dirt:
                    activeParticle = ActiveParticle.DirtLanding;
                    Debug.Log("dirt land");
                    break;
            }
        }

        private void ResetToRunningParticle()
        {
            switch (tileTypeDetector.GetTileType())
            {
                case TileType.None: break;
                case TileType.Grass:
                    activeParticle = ActiveParticle.GrassRunning;

                    break;
                case TileType.Dirt:
                    activeParticle = ActiveParticle.DirtRunning;
                    break;
            }
        }

        private void SetVFXSpawnPosition(Vector3 position)
        {
            burstParticleVFX.SetVector3("SpawnPosition", position);
        }

        public void InitializePC(Component sender, object data)
        {
            playerController = (PlayerController)data;
        }
    }
}
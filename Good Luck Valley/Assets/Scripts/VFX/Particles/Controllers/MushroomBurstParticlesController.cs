using GoodLuckValley.Events;
using GoodLuckValley.Mushroom;
using GoodLuckValley.Player.Control;
using GoodLuckValley.VFX.ScriptableObjects;
using GoodLuckValley.World.Tiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class MushroomBurstParticlesController : MonoBehaviour
{
    // Enum for currently active particle
    enum ActiveParticle
    {
        ShroomGrow,
        ShroomBounce,
        None
    }


    #region REFERENCES
    [Header("General")]
    [SerializeField] private DetectTileType detectTileType;
    private VisualEffect burstParticleVFX;

    [Header("Particle Data")]
    [SerializeField] private VFXBurstParticleData shroomBounceData;
    [SerializeField] private VFXBurstParticleData shroomSpawnData;
    #endregion

    #region FIELDS
    [SerializeField] private ActiveParticle activeParticle;
    private GameObject activeMushroom;
    #endregion

    private void Awake()
    {
        activeParticle = ActiveParticle.None;
        burstParticleVFX = GetComponent<VisualEffect>();
    }

    private void Update()
    {
        // Check to make sure the necessary references are not null
        if (activeParticle != ActiveParticle.None)
        {
            // Check currently active particle
            switch (activeParticle)
            {
                // No particle active
                case ActiveParticle.None:
                    break;

                case ActiveParticle.ShroomGrow:
                    // Send VFX data
                    SendVFXData(shroomSpawnData);

                    // Send the VFX spawn position
                    SendVFXSpawnPosition(activeMushroom.transform.position);

                    // Play the particle
                    burstParticleVFX.Play();

                    // Reset the active particle
                    ResetToNone();
                    break;

                case ActiveParticle.ShroomBounce:
                    // Send VFX data
                    SendVFXData(shroomBounceData);

                    // Send the VFX spawn position
                    SendVFXSpawnPosition(activeMushroom.transform.position);

                    // Play the particle
                    burstParticleVFX.Play();

                    // Reset the active particle
                    ResetToNone();
                    break;
            }
        }
    }

    #region SENDING DATA TO GPU
    private void SendVFXData(VFXBurstParticleData data)
    {
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

    #region MUSHROOM PARTICLES HANDLERS
    /// <summary>
    /// Handles setting active particle information for shroom bounce particles via game event
    /// </summary>
    /// <param name="sender"> The component sending the event information </param>
    /// <param name="data"> The data being sent </param>
    public void HandleShroomBounceParticles(Component sender, object data)
    {
        MushroomBounce.BounceData bounceData = (MushroomBounce.BounceData)data;
        activeMushroom = bounceData.Mushroom;
        activeParticle = ActiveParticle.ShroomBounce;
    }

    /// <summary>
    /// Handles tile detection and setting active particle information for shroom bounce particles via game event
    /// </summary>
    /// <param name="sender"> The component sending the event information </param>
    /// <param name="data"> The data being sent </param>
    public void HandleShroomSpawnParticles(Component sender, object data)
    {
        // Initialize active mushroom for spawn positions
        activeMushroom = (GameObject)data;

        // Set start of detect tile type raycat
        detectTileType.RaycastStart = activeMushroom.transform.position;

        switch (detectTileType.CheckTileType()) 
        {
            case TileType.Grass:
                // Set particle to grass
                activeParticle = ActiveParticle.ShroomGrow;
                break;
            case TileType.Dirt:
                // Set particle to dirt
                activeParticle = ActiveParticle.ShroomGrow;
                break;
        }
    }
    #endregion

    #region HELPERS
    private void ResetToNone()
    {
        activeParticle = ActiveParticle.None;
    }
    #endregion
}
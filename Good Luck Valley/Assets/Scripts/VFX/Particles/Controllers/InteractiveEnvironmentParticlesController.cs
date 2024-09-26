using GoodLuckValley.VFX;
using GoodLuckValley.VFX.Particles.Controllers;
using GoodLuckValley.VFX.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class InteractiveEnvironmentParticlesController : MonoBehaviour
{
    #region FIELDS
    private VisualEffect upwardsBurstParticle;
    #endregion

    #region REFERENCES
    [SerializeField] private VFXBurstParticleData bluePlantParticleData;
    [SerializeField] private VFXBurstParticleData purplePlantParticleData;
    #endregion

    private void Awake()
    {
        upwardsBurstParticle = GetComponent<VisualEffect>();
    }

    public void PlayParticleEffect(Component sender, object data)
    {
        SimpleInteractiveEnvironmentTrigger.InteractiveEnvironmentData environmentData = (SimpleInteractiveEnvironmentTrigger.InteractiveEnvironmentData)data;
        upwardsBurstParticle.SetVector3("SpawnPosition", environmentData.Position);
        switch(environmentData.PlantType)
        {
            case SimpleInteractiveEnvironmentTrigger.PlantType.Blue:
                SendVFXDataToGPU(bluePlantParticleData);
                break;
            case SimpleInteractiveEnvironmentTrigger.PlantType.Purple:
                SendVFXDataToGPU(purplePlantParticleData);
                break;
        }
        upwardsBurstParticle.Play();
    }

    private void SendVFXDataToGPU(VFXBurstParticleData data)
    {
        // Spawn data
        upwardsBurstParticle.SetFloat("SpawnCount", data.spawnCount);

        // Initialize data
        upwardsBurstParticle.SetFloat("Min X Velocity", data.minXVelocity);
        upwardsBurstParticle.SetFloat("Max X Velocity", data.maxXVelocity);
        upwardsBurstParticle.SetFloat("Min Y Velocity", data.minYVelocity);
        upwardsBurstParticle.SetFloat("Max Y Velocity", data.maxYVelocity);
        upwardsBurstParticle.SetFloat("Min Lifetime", data.minLifetime);
        upwardsBurstParticle.SetFloat("Max Lifetime", data.maxLifetime);
        upwardsBurstParticle.SetFloat("Min Size", data.minSize);
        upwardsBurstParticle.SetFloat("Max Size", data.maxSize);
        upwardsBurstParticle.SetFloat("Min Starting Angle", data.minStartingAngle);
        upwardsBurstParticle.SetFloat("Max Starting Angle", data.maxStartingAngle);
        upwardsBurstParticle.SetVector3("Min Position Offset", data.minPositionOffset);
        upwardsBurstParticle.SetVector3("Max Position Offset", data.maxPositionOffset);

        // Update data
        upwardsBurstParticle.SetFloat("Gravity", data.gravity);
        upwardsBurstParticle.SetFloat("Min Linear Drag Coefficient", data.minLinearDragCoefficient);
        upwardsBurstParticle.SetFloat("Max Linear Drag Coefficient", data.maxLinearDragCoefficient);
        upwardsBurstParticle.SetFloat("Turb Intensity", data.turbulenceIntensity);
        upwardsBurstParticle.SetFloat("Turb Drag", data.turbulenceDrag);
        upwardsBurstParticle.SetFloat("Turb Frequency", data.turbulenceFrequency);
        upwardsBurstParticle.SetFloat("Add Z Angle", data.addZAngle);
        upwardsBurstParticle.SetFloat("Min Add Angle Scalar", data.minAddAngleScalar);
        upwardsBurstParticle.SetFloat("Max Add Angle Scalar", data.maxAddAngleScalar);

        // Output data
        upwardsBurstParticle.SetTexture("MainTex", data.mainTex);
        upwardsBurstParticle.SetAnimationCurve("Mult Size / Life", data.multiplySizeOverLife);
        upwardsBurstParticle.SetGradient("Color / Life", data.colorOverLife);

    }
}
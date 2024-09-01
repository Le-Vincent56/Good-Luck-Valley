using GoodLuckValley.VFX;
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
                Debug.Log("blue");
                break;
            case SimpleInteractiveEnvironmentTrigger.PlantType.Purple:
                Debug.Log("purple");
                break;
        }
        upwardsBurstParticle.Play();
    }
}
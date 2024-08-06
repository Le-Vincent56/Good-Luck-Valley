using GoodLuckValley.VFX.ScriptableObjects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.VFX.Particles
{
    enum ActiveParticle
    {
        GrassRunning,
        GrassJumping,
        GrassLanding,
        DirtRunning,
        DirtJumping,
        DirtLanding
    }

    public class ReactiveBurstParticleController : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] private VFXBurstParticleData grassRunningData;
        [SerializeField] private VFXBurstParticleData grassLandData;
        [SerializeField] private VFXBurstParticleData grassJumpData;
        [SerializeField] private VFXBurstParticleData dirtRunningData;
        [SerializeField] private VFXBurstParticleData dirtLandData;
        [SerializeField] private VFXBurstParticleData dirtJumpData;
        #endregion

        #region FIELDS
        [SerializeField] private ActiveParticle activeParticle;
        #endregion


    }
}
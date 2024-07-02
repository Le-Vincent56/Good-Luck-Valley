using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace GoodLuckValley.VFX
{
    public class ConstantParticleController : MonoBehaviour
    {
        // References
        private VisualEffect vfx;

        private void Update()
        {
            //vfx.SetVector2("WorldSpawnPosition", transform.position);
        }

        public void InitiateAndPlayEffect(VisualEffect vfx)
        {
            this.vfx = vfx;
            vfx.Play();
        }

        public void StopEffect()
        {
            vfx.Stop();
        }

        private void OnDisable()
        {
            StopEffect();
        }
    }
}
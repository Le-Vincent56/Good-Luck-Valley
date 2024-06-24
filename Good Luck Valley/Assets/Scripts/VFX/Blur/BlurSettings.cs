using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace GoodLuckValley.VFX
{
    [System.Serializable, VolumeComponentMenu("Blur")]
    public class BlurSettings : VolumeComponent, IPostProcessComponent
    {
        // Fields
        [Tooltip("Standard eviateon (spread) of the blur. Grid size is approx. 3x larger.")]
        public ClampedFloatParameter strength = new ClampedFloatParameter(0.0f, 0.0f, 15.0f);

        public bool IsActive()
        {
            return (strength.value > 0.0f) && active;
        }

        // Marked obsolete
        public bool IsTileCompatible()
        {
            return false;
        }
    }
}
using UnityEngine.Rendering.Universal;

namespace GoodLuckValley.VFX.Lights
{
    public interface ILightAnimator
    {
        void Initialize(Light2D targetLight);
        void UpdateAnimation(float deltaTime);
        void SetEnabled(bool enabled);
        void ResetToBaseValues();
    }
}
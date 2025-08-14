using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GoodLuckValley.VFX.Lights
{
    public class OutwardAnimatedLight : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private Light2D targetLight;

        [Header("Tweening Variables")] 
        [SerializeField] private float outIntensity;
        [SerializeField] private float outOuterRadius;
        [SerializeField] private float outInnerRadius;
        [SerializeField] private float inIntensity;
        [SerializeField] private float inOuterRadius;
        [SerializeField] private float inInnerRadius;
        [SerializeField] private float outDuration;
        [SerializeField] private float inDuration;
        private Sequence outSequence;
        private Sequence inSequence;

        private void OnDestroy()
        {
            outSequence?.Kill();
        }
        
        public void Animate()
        {
            // Kill the sequencse if they exist already
            outSequence?.Kill();
            inSequence?.Kill();
            
            // Create the out sequence
            outSequence = DOTween.Sequence();

            Tween outIntensityTween = DOTween.To(
                () => targetLight.intensity,
                x => targetLight.intensity = x,
                outIntensity,
                outDuration
            );
            
            Tween outOuterRadiusTween = DOTween.To(
                () => targetLight.pointLightOuterRadius,
                x => targetLight.pointLightOuterRadius = x,
                outOuterRadius,
                outDuration
            );
            
            Tween outInnerRadiusTween = DOTween.To(
                () => targetLight.pointLightInnerRadius,
                x => targetLight.pointLightInnerRadius = x,
                outInnerRadius,
                outDuration
            );

            // Append the out-tweens
            outSequence.Append(outIntensityTween);
            outSequence.Join(outOuterRadiusTween);
            outSequence.Join(outInnerRadiusTween);

            // Create the in sequence
            inSequence = DOTween.Sequence();
            
            Tween inIntensityTween = DOTween.To(
                () => targetLight.intensity,
                x => targetLight.intensity = x,
                inIntensity,
                inDuration
            );
            
            Tween inOuterRadiusTween = DOTween.To(
                () => targetLight.pointLightOuterRadius,
                x => targetLight.pointLightOuterRadius = x,
                inOuterRadius,
                inDuration
            );
            
            Tween inInnerRadiusTween = DOTween.To(
                () => targetLight.pointLightInnerRadius,
                x => targetLight.pointLightInnerRadius = x,
                inInnerRadius,
                inDuration
            );
            
            // Append the in-tweens
            inSequence.Append(inIntensityTween);
            inSequence.Join(inOuterRadiusTween);
            inSequence.Join(inInnerRadiusTween);
            
            // Link the sequences today and play
            outSequence.OnComplete(() => inSequence.Play());
            outSequence.Play();
        }
    }
}

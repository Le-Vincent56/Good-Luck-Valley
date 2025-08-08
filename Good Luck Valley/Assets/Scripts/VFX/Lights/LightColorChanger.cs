using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GoodLuckValley
{
    public class LightColorChanger : MonoBehaviour
    {
        private Light2D targetLight;
        
        [Header("Tweening Fields")] 
        [SerializeField] private float tweenDuration;
        [SerializeField] private Color finalValue;
        private Tween colorTween;

        private void Awake()
        {
            targetLight = GetComponent<Light2D>();
        }
        
        private void OnDestroy()
        {
            colorTween?.Kill();
        }

        /// <summary>
        /// Change the color of a light
        /// </summary>
        public void ChangeColor()
        {
            // Kill the existing tween if it exists
            colorTween?.Kill();

            // Change the color of a light
            colorTween = DOTween.To(
                () => targetLight.color,
                x => targetLight.color = x,
                finalValue,
                tweenDuration
            );
        }
    }
}

using DG.Tweening;
using GoodLuckValley.Events;
using GoodLuckValley.Events.Cinematics;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.World.Cinematics
{
    public class CinematicCutToBlack : MonoBehaviour
    {
        private Image cutImage;

        private Tween cutTween;
        private EventBinding<CutToBlack> onCutToBlack;

        private void Awake()
        {
            // Get the Image component
            cutImage = GetComponent<Image>();
        }

        private void OnEnable()
        {
            onCutToBlack = new EventBinding<CutToBlack>(Cut);
            EventBus<CutToBlack>.Register(onCutToBlack);
        }

        private void OnDisable()
        {
            EventBus<CutToBlack>.Deregister(onCutToBlack);
        }

        private void OnDestroy()
        {
            // Kill the Cut Tween if it exists
            cutTween?.Kill();
        }
        
        /// <summary>
        /// Use tweening to instantly cut to black
        /// </summary>
        private void Cut()
        {
            // Kill the Cut Tween if it exists
            cutTween?.Kill();

            // Cut the loading image
            cutTween = cutImage.DOFade(1f, 0f);
        }
    }
}

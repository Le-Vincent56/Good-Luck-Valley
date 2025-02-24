using Cinemachine;
using DG.Tweening;
using GoodLuckValley.Player.Movement;
using GoodLuckValley.World.Triggers;
using UnityEngine;

namespace GoodLuckValley
{
    public class OneWayCameraScaler : BaseTrigger
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;

        [Header("Tweening Variables")]
        [SerializeField] private float toScale;
        [SerializeField] private float scaleDuration;
        [SerializeField] private Ease easeType;
        private Tween scaleTween;

        private void OnDestroy()
        {
            // Kill the Scale tween if it exists
            scaleTween?.Kill();
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            // Exit case - the collision object is not the Player
            if (!other.TryGetComponent(out PlayerController controller)) return;

            // Translate to the given offset
            Scale(toScale, scaleDuration, easeType);
        }

        /// <summary>
        /// Handle scale tweening for the Cinemachine Orthographic Size
        /// </summary>
        private void Scale(float endValue, float duration, Ease easeType, TweenCallback onComplete = null)
        {
            // Kill the Scale Tween, if it exists
            scaleTween?.Kill();

            // Tween the orthographic size to the endValue using the duration and set an Ease
            scaleTween = DOTween.To(
                () => virtualCamera.m_Lens.OrthographicSize,
                    x => virtualCamera.m_Lens.OrthographicSize = x,
                    endValue,
                    duration
            );

            // Set the ease type
            scaleTween.SetEase(easeType);

            // Exit case - no completion action was given
            if (onComplete == null) return;

            // Hook up completion actions
            scaleTween.onComplete += onComplete;
        }
    }
}

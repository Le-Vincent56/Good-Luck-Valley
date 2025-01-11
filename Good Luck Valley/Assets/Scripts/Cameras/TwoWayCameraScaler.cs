using Cinemachine;
using DG.Tweening;
using GoodLuckValley.World.Triggers;
using UnityEngine;

namespace GoodLuckValley.Cameras
{
    public class TwoWayCameraScaler : TwoWayTrigger
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        [SerializeField] private float rightOrthographicSize;
        [SerializeField] private float leftOrthographicSize;

        [Header("Tweening Variables")]
        [SerializeField] private float scaleDuration;
        [SerializeField] private Ease easeType;
        private Tween scaleTween;

        protected override void OnRight() => Scale(rightOrthographicSize, scaleDuration, easeType);
        protected override void OnLeft() => Scale(leftOrthographicSize, scaleDuration, easeType);

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

using Cinemachine;
using DG.Tweening;
using GoodLuckValley.World.Triggers;
using UnityEngine;

namespace GoodLuckValley.Cameras
{
    public class TwoWayCameraOffsetTrigger : TwoWayTrigger
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        private CinemachineFramingTransposer framingTransposer;

        [Header("Tweening Variables")]
        [SerializeField] private Vector3 leftOffset;
        [SerializeField] private Vector3 rightOffset;
        [SerializeField] private float translateDuration;
        [SerializeField] private Ease easeType;
        private Tween translateTween;

        protected override void Awake()
        {
            // Call the parent awake
            base.Awake();

            // Get the Framing Trasnposer
            framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }

        protected override void OnRight() => Translate(rightOffset, translateDuration, easeType);
        protected override void OnLeft() => Translate(leftOffset, translateDuration, easeType);

        /// <summary>
        /// Handle translate tweening for the Cinemachine Framing Transposer Tracked Object Offset
        /// </summary>
        private void Translate(Vector3 endValue, float duration, Ease easeType, TweenCallback onComplete = null)
        {
            // Kill the Translate Tween, if it exists
            translateTween?.Kill();

            // Tween the m_TrackedObjectOffset to the endValue using the duration and set an Ease
            translateTween = DOTween.To(
                () => framingTransposer.m_TrackedObjectOffset,
                    x => framingTransposer.m_TrackedObjectOffset = x,
                    endValue,
                    duration
            );

            // Set the ease type
            translateTween.SetEase(easeType);

            // Exit case - no completion action was given
            if (onComplete == null) return;

            // Hook up completion actions
            translateTween.onComplete += onComplete;
        }
    }
}

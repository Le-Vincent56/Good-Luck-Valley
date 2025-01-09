using Cinemachine;
using DG.Tweening;
using GoodLuckValley.Player.Movement;
using UnityEngine;

namespace GoodLuckValley.Cameras
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class TwoWayCameraOffsetTrigger : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        private CinemachineFramingTransposer framingTransposer;
        private Vector3 center;

        [Header("Tweening Variables")]
        [SerializeField] private Vector3 leftOffset;
        [SerializeField] private Vector3 rightOffset;
        [SerializeField] private float translateDuration;
        [SerializeField] private Ease easeType;
        private Tween translateTween;

        private void Awake()
        {
            // Get the Framing Trasnposer
            framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();

            // Set the center of the trigger
            Bounds bounds = GetComponent<BoxCollider2D>().bounds;
            center = bounds.center;
        }


        private void OnTriggerExit2D(Collider2D other)
        {
            // Exit case - the collision object is not the Player
            if (!other.TryGetComponent(out PlayerController controller)) return;

            // Get the direction from the controller to the player
            Vector2 enterDirection = controller.transform.position - center;
            int enterDirectionX = (int)Mathf.Sign(enterDirection.x);

            // Check if exiting from the right
            if (enterDirectionX == 1)
                // Prioritize the right camera
                Translate(rightOffset, translateDuration, easeType);
            // Else, check if exiting from the left
            else if (enterDirectionX == -1)
                Translate(leftOffset, translateDuration, easeType);
        }

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

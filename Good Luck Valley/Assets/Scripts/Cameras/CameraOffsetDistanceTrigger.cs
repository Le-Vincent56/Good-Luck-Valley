using Cinemachine;
using GoodLuckValley.World.Triggers;
using UnityEngine;

namespace GoodLuckValley.Cameras
{
    public class CameraOffsetDistanceTrigger : Vector2DistanceBasedTrigger
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        private CinemachineFramingTransposer framingTransposer;

        protected override void Awake()
        {
            base.Awake();

            // Get the Framing Transposer
            framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }

        protected override void OnTriggerStay2D(Collider2D collision)
        {
            // Calculate through the parent class
            base.OnTriggerStay2D(collision);

            // Set the Orthographic Size using the current distance value
            framingTransposer.m_TrackedObjectOffset = currentDistanceValue;
        }
    }
}

using Cinemachine;
using GoodLuckValley.World.Triggers;
using UnityEngine;

namespace GoodLuckValley.Cameras
{
    public enum BodyType
    {
        None,
        FramingTransposer,
        Dolly
    }

    public class CameraOffsetDistanceTrigger : Vector2DistanceBasedTrigger
    {
        [SerializeField] private BodyType bodyType;
        [SerializeField] private CinemachineVirtualCamera virtualCamera;
        private CinemachineComponentBase component;

        protected override void Awake()
        {
            base.Awake();

            // Extract the Cinemachine Component
            switch (bodyType)
            {
                case BodyType.None:
                    break;

                case BodyType.FramingTransposer:
                    component = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
                    break;

                case BodyType.Dolly:
                    component = virtualCamera.GetCinemachineComponent<CinemachineTrackedDolly>();
                    break;
            }
        }

        protected override void OnTriggerStay2D(Collider2D collision)
        {
            // Calculate through the parent class
            base.OnTriggerStay2D(collision);

            // Extract the Cinemachine Component
            switch (bodyType)
            {
                case BodyType.None:
                    break;

                case BodyType.FramingTransposer:
                    // Cast to a Framing Transposer
                    CinemachineFramingTransposer framingTransposer = component as CinemachineFramingTransposer;

                    // Set the tracked object offset using the current distance value
                    framingTransposer.m_TrackedObjectOffset = currentDistanceValue;
                    break;

                case BodyType.Dolly:
                    // Cast to a Tracked Dolly
                    CinemachineTrackedDolly dolly = component as CinemachineTrackedDolly;

                    // Set the tracked object offset using the current distance value
                    dolly.m_PathOffset = currentDistanceValue;
                    break;
            }
        }
    }
}

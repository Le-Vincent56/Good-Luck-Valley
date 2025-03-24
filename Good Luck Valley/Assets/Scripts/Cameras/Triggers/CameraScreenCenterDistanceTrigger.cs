using Cinemachine;
using GoodLuckValley.World.Triggers;
using UnityEngine;

namespace GoodLuckValley.Cameras
{
    public class CameraScreenCenterDistanceTrigger : Vector2DistanceBasedTrigger
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

            // Set the screen centerp position using the current distance value
            framingTransposer.m_ScreenX = currentDistanceValue.x;
            framingTransposer.m_ScreenY = currentDistanceValue.y;
        }
    }
}

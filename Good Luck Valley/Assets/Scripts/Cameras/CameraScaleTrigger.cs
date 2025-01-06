using Cinemachine;
using GoodLuckValley.World.Triggers;
using UnityEngine;

namespace GoodLuckValley.Cameras
{
    public class CameraScaleTrigger : FloatDistanceBasedTrigger
    {
        [SerializeField] private CinemachineVirtualCamera virtualCamera;

        protected override void OnTriggerStay2D(Collider2D collision)
        {
            // Calculate through the parent class
            base.OnTriggerStay2D(collision);

            // Set the Orthographic Size using the current distance value
            virtualCamera.m_Lens.OrthographicSize = currentDistanceValue;
        }
    }
}

using Cinemachine;
using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.World.Triggers;
using UnityEngine;

namespace GoodLuckValley.Cameras
{
    public class TwoWayCameraTrigger : TwoWayTrigger
    {
        private CameraController cameraController;
        [SerializeField] private CinemachineVirtualCamera leftCamera;
        [SerializeField] private CinemachineVirtualCamera rightCamera;
        [SerializeField] private CinemachineVirtualCamera upCamera;
        [SerializeField] private CinemachineVirtualCamera downCamera;

        private void Start()
        {
            // Get the Camera Controller as a servicve
            cameraController = ServiceLocator.ForSceneOf(this).Get<CameraController>();
        }

        protected override void OnRight() => cameraController.PrioritizeCamera(rightCamera);
        protected override void OnLeft() => cameraController.PrioritizeCamera(leftCamera);
        protected override void OnUp() => cameraController.PrioritizeCamera(upCamera);
        protected override void OnDown() => cameraController.PrioritizeCamera(downCamera);
    }
}

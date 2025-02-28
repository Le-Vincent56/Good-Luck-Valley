using Cinemachine;
using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Events;
using GoodLuckValley.Events.Fireflies;
using UnityEngine;

namespace GoodLuckValley.Cameras.Triggers
{
    public class CameraLanternTrigger : MonoBehaviour
    {
        [Header("References")]
        private CameraController cameraController;
        [SerializeField] private CinemachineVirtualCamera cameraToPrioritize;

        [Header("Fields")]
        [SerializeField] private int channel;

        private EventBinding<ActivateLantern> onActivateLantern;

        private void OnEnable()
        {
            onActivateLantern = new EventBinding<ActivateLantern>(PrioritizeCamera);
            EventBus<ActivateLantern>.Register(onActivateLantern);
        }

        private void OnDisable()
        {
            EventBus<ActivateLantern>.Deregister(onActivateLantern);
        }

        private void Start()
        {
            // Get the camera Controller
            cameraController = ServiceLocator.ForSceneOf(this).Get<CameraController>();
        }

        private void PrioritizeCamera(ActivateLantern eventData)
        {
            // Exit case - the channel doesn't match
            if (eventData.Channel != channel) return;

            // Prioritize the camera
            cameraController.PrioritizeCamera(cameraToPrioritize);
        }
    }
}

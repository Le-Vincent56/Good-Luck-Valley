using UnityEngine;
using Cinemachine;
using static UnityEngine.RuleTile.TilingRuleOutput;

namespace GoodLuckValley.Cameras
{
    [System.Serializable]
    public class CameraInspectorObjects
    {
        public bool swapCameras = false;
        public bool panCameraOnContact = false;

        [HideInInspector] public CinemachineVirtualCamera cameraOnLeft;
        [HideInInspector] public CinemachineVirtualCamera cameraOnRight;

        [HideInInspector] public Vector2 panDirection;
        [HideInInspector] public float panDistance = 3f;
        [HideInInspector] public float panTime = 0.35f;
    }
}
using UnityEngine;
using UnityEditor;
using GoodLuckValley.Entity;
using GoodLuckValley.Entities;

namespace GoodLuckValley.Cameras
{
    public class CameraControlTrigger : MonoBehaviour
    {
        [Header("References")]
        private AreaCollider triggerCollider;

        [Header("Fields")]
        public CameraInspectorObjects cameraInspectorObjects;
        [HideInInspector] public Vector2 localPanPoint;
        [HideInInspector] public Vector2 globalPanPoint;

        private void Awake()
        {
            triggerCollider = GetComponent<AreaCollider>();
        }

        private void OnEnable()
        {
            triggerCollider.OnTriggerEnter += EnterTrigger;
            triggerCollider.OnTriggerExit += ExitTrigger;
        }

        private void Start()
        {
            // Convert local pan point to a global pan point
            globalPanPoint = localPanPoint + (Vector2)transform.position;

            // Set the pan direction
            cameraInspectorObjects.panDirection = (globalPanPoint - (Vector2)transform.position).normalized;
        }

        private void OnDisable()
        {
            triggerCollider.OnTriggerEnter -= EnterTrigger;
            triggerCollider.OnTriggerExit -= ExitTrigger;
        }

        private void EnterTrigger(GameObject gameObj)
        {
            if (cameraInspectorObjects.panCameraOnContact)
            {
                CameraManager.Instance.PanCameraOnContact(
                    cameraInspectorObjects.panDistance,
                    cameraInspectorObjects.panTime,
                    cameraInspectorObjects.panDirection,
                    false
                );
            }
        }

        private void ExitTrigger(GameObject gameObj)
        {
            // Check if the trigger should swap cameras and has valid cameras to swap between
            if(cameraInspectorObjects.swapCameras && 
                cameraInspectorObjects.cameraOnLeft != null && 
                cameraInspectorObjects.cameraOnRight != null)
            {
                Vector2 exitDirection = (gameObj.transform.position - triggerCollider.Bounds.center).normalized;

                CameraManager.Instance.SwapCamera(cameraInspectorObjects.cameraOnLeft, cameraInspectorObjects.cameraOnRight, exitDirection);
            }

            if (cameraInspectorObjects.panCameraOnContact)
            {
                CameraManager.Instance.PanCameraOnContact(
                    cameraInspectorObjects.panDistance,
                    cameraInspectorObjects.panTime,
                    cameraInspectorObjects.panDirection,
                    true
                );
            }
        }

        private void OnDrawGizmos()
        {
            if (localPanPoint != Vector2.zero && cameraInspectorObjects.panCameraOnContact)
            {
                Gizmos.color = Color.red;
                float size = 0.1f;

                Vector2 globalPanPointPos = (Application.isPlaying) ? globalPanPoint : localPanPoint + (Vector2)transform.position;

                // Draw a circle for points
                Gizmos.DrawSphere(globalPanPointPos, size);
                Gizmos.DrawLine(transform.position, globalPanPointPos);
            }
        }
    }
}
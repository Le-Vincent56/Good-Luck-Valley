using UnityEngine;
using GoodLuckValley.World.AreaTriggers;
using GoodLuckValley.Events;
namespace GoodLuckValley.Cameras
{
    [RequireComponent(typeof(AreaCollider))]
    public class CameraControlTrigger : MonoBehaviour
    {
        public enum Direction
        {
            LeftRight,
            UpDown
        }

        public enum UpdateDirection
        {
           Left,
           Right,
           Up,
           Down
        }

        [Header("Events")]
        [SerializeField] private GameEvent onSetUpdatePosition;

        [Header("References")]
        private AreaCollider triggerCollider;

        [Header("Fields")]
        [SerializeField] private bool usesNonStaticObj;
        [SerializeField] private UpdateDirection updateDirection;
        [SerializeField] private Direction direction;
        public CameraInspectorObjects cameraInspectorObjects;
        private Vector2 localPanPoint;
        private Vector2 globalPanPoint;

        public GameEvent OnSetUpdatePosition
        {
            get => onSetUpdatePosition;
            set => onSetUpdatePosition = value;
        }

        public bool UsesNonStaticObj
        {
            get => usesNonStaticObj;
            set => usesNonStaticObj = value;
        }

        public UpdateDirection UpdateNonStaticDirection
        {
            get => updateDirection;
            set => updateDirection = value;
        }

        public Direction TriggerDirection
        {
            get => direction;
            set => direction = value;
        }

        public Vector2 LocalPanPoint
        {
            get => localPanPoint;
            set => localPanPoint = value;
        }

        public Vector2 GlobalPanPoint
        {
            get => globalPanPoint;
            set => globalPanPoint = value;
        }

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
            if(usesNonStaticObj && onSetUpdatePosition != null)
                onSetUpdatePosition.Raise(this, true);

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
            switch (direction)
            {
                case Direction.LeftRight:
                    if (cameraInspectorObjects.swapCameras &&
                        cameraInspectorObjects.cameraOnLeft != null &&
                        cameraInspectorObjects.cameraOnRight != null)
                    {
                        Vector2 exitDirection = (gameObj.transform.position - triggerCollider.Bounds.center).normalized;

                        CameraManager.Instance.SwapCamera(cameraInspectorObjects.cameraOnLeft, cameraInspectorObjects.cameraOnRight, exitDirection, true);

                        if (usesNonStaticObj && onSetUpdatePosition != null)
                        {
                            if (exitDirection.x < 0f && updateDirection == UpdateDirection.Left)
                            {
                                onSetUpdatePosition.Raise(this, false);
                            }
                            else if (exitDirection.x > 0f && updateDirection == UpdateDirection.Right)
                            {
                                onSetUpdatePosition.Raise(this, false);
                            }
                        }
                    }
                    break;

                case Direction.UpDown:
                    if (cameraInspectorObjects.swapCameras &&
                        cameraInspectorObjects.cameraOnTop != null &&
                        cameraInspectorObjects.cameraOnBottom != null)
                    {
                        Vector2 exitDirection = (gameObj.transform.position - triggerCollider.Bounds.center).normalized;

                        CameraManager.Instance.SwapCamera(cameraInspectorObjects.cameraOnTop, cameraInspectorObjects.cameraOnBottom, exitDirection, false);

                        if(usesNonStaticObj && onSetUpdatePosition != null)
                        {
                            if (exitDirection.y < 0f && updateDirection == UpdateDirection.Down)
                            {
                                onSetUpdatePosition.Raise(this, false);
                            }
                            else if (exitDirection.y > 0f && updateDirection == UpdateDirection.Up)
                            {
                                onSetUpdatePosition.Raise(this, false);
                            }
                        }
                    }
                    break;
            }

            if(usesNonStaticObj && onSetUpdatePosition != null)
                onSetUpdatePosition.Raise(this, updateDirection);


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
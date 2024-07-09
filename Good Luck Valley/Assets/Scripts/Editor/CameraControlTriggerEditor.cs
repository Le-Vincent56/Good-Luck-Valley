using Cinemachine;
using GoodLuckValley.Cameras;
using GoodLuckValley.Events;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraControlTrigger))]
public class CameraControlTriggerEditor : Editor
{
    CameraControlTrigger cameraControlTrigger;
    private static readonly CameraControlTrigger.UpdateDirection[] AllowedDirectionsVert = { CameraControlTrigger.UpdateDirection.Up, CameraControlTrigger.UpdateDirection.Down };
    private static readonly CameraControlTrigger.UpdateDirection[] AllowedDirectionsHor = { CameraControlTrigger.UpdateDirection.Left, CameraControlTrigger.UpdateDirection.Right };

    private void OnEnable()
    {
        cameraControlTrigger = (CameraControlTrigger)target;
    }

    public override void OnInspectorGUI()
    {
        // Show camera variables
        if(cameraControlTrigger.cameraInspectorObjects.swapCameras)
        {
            EditorGUILayout.LabelField("Follow Parameters", EditorStyles.boldLabel);

            cameraControlTrigger.UsesNonStaticObj = EditorGUILayout.Toggle("Uses a Non-Static Object", cameraControlTrigger.UsesNonStaticObj);
            if(cameraControlTrigger.UsesNonStaticObj)
            {
                cameraControlTrigger.OnSetUpdatePosition = EditorGUILayout.ObjectField(
                    "On Set Update Position", 
                    cameraControlTrigger.OnSetUpdatePosition, 
                    typeof(GameEvent), true
                ) as GameEvent;

                cameraControlTrigger.UpdateNonStaticDirection = (CameraControlTrigger.UpdateDirection)EditorGUILayout.EnumPopup("End Update Direction", cameraControlTrigger.UpdateNonStaticDirection);
            }

            EditorGUILayout.Space();


            EditorGUILayout.LabelField("Trigger Parameters", EditorStyles.boldLabel);
            cameraControlTrigger.TriggerDirection = (CameraControlTrigger.Direction)EditorGUILayout.EnumPopup("Camera Direction", cameraControlTrigger.TriggerDirection);

            switch (cameraControlTrigger.TriggerDirection)
            {
                case CameraControlTrigger.Direction.LeftRight:
                    cameraControlTrigger.cameraInspectorObjects.cameraOnLeft = EditorGUILayout.ObjectField(
                        "Camera on Left",
                        cameraControlTrigger.cameraInspectorObjects.cameraOnLeft,
                        typeof(CinemachineVirtualCamera), true
                    ) as CinemachineVirtualCamera;

                    cameraControlTrigger.cameraInspectorObjects.cameraOnRight = EditorGUILayout.ObjectField(
                        "Camera on Right",
                        cameraControlTrigger.cameraInspectorObjects.cameraOnRight,
                        typeof(CinemachineVirtualCamera), true
                    ) as CinemachineVirtualCamera;
                    break;

                case CameraControlTrigger.Direction.UpDown:
                    cameraControlTrigger.cameraInspectorObjects.cameraOnTop = EditorGUILayout.ObjectField(
                        "Camera on Top",
                        cameraControlTrigger.cameraInspectorObjects.cameraOnTop,
                        typeof(CinemachineVirtualCamera), true
                    ) as CinemachineVirtualCamera;

                    cameraControlTrigger.cameraInspectorObjects.cameraOnBottom = EditorGUILayout.ObjectField(
                        "Camera on Bottom",
                        cameraControlTrigger.cameraInspectorObjects.cameraOnBottom,
                        typeof(CinemachineVirtualCamera), true
                    ) as CinemachineVirtualCamera;
                    break;
            }
        }

        // Show pan variables
        if (cameraControlTrigger.cameraInspectorObjects.panCameraOnContact)
        {
            cameraControlTrigger.cameraInspectorObjects.panDirection = EditorGUILayout.Vector2Field(
                "Camera Pan Direction",
                cameraControlTrigger.cameraInspectorObjects.panDirection
            );

            cameraControlTrigger.cameraInspectorObjects.panDistance = EditorGUILayout.FloatField(
                "Pan Distance", 
                cameraControlTrigger.cameraInspectorObjects.panDistance
            );

            cameraControlTrigger.cameraInspectorObjects.panTime = EditorGUILayout.FloatField(
                "Pan Time",
                cameraControlTrigger.cameraInspectorObjects.panTime
            );

            cameraControlTrigger.LocalPanPoint = EditorGUILayout.Vector2Field(
                "Local Pan Point",
                cameraControlTrigger.LocalPanPoint
            );

            cameraControlTrigger.GlobalPanPoint = EditorGUILayout.Vector2Field(
                "Global Pan Point",
                cameraControlTrigger.GlobalPanPoint
            );
        }

        // Check for GUI changes
        if(GUI.changed)
        {
            EditorUtility.SetDirty(cameraControlTrigger);
        }
    }
}

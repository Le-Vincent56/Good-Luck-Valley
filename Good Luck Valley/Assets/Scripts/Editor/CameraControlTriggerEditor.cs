using Cinemachine;
using GoodLuckValley.Cameras;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraControlTrigger))]
public class CameraControlTriggerEditor : Editor
{
    CameraControlTrigger cameraControlTrigger;

    private void OnEnable()
    {
        cameraControlTrigger = (CameraControlTrigger)target;
    }

    public override void OnInspectorGUI()
    {
        // Draw the default inspector
        DrawDefaultInspector();

        // Show camera variables
        if(cameraControlTrigger.cameraInspectorObjects.swapCameras)
        {
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
        }

        // Show pan variables
        if(cameraControlTrigger.cameraInspectorObjects.panCameraOnContact)
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

            cameraControlTrigger.localPanPoint = EditorGUILayout.Vector2Field(
                "Local Pan Point",
                cameraControlTrigger.localPanPoint
            );

            cameraControlTrigger.globalPanPoint = EditorGUILayout.Vector2Field(
                "Global Pan Point",
                cameraControlTrigger.globalPanPoint
            );
        }

        // Check for GUI changes
        if(GUI.changed)
        {
            EditorUtility.SetDirty(cameraControlTrigger);
        }
    }
}

using GoodLuckValley.Cameras;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class AmbientParticlesController : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] private CameraManager cameraManager;
    [SerializeField] private VisualEffect ambientDustFG;
    [SerializeField] private VisualEffect ambientDustMG;
    [SerializeField] private VisualEffect ambientLeavesFG;
    [SerializeField] private VisualEffect ambientLeavesMG;
    #endregion

    #region FIELDS

    #endregion

    private void Update()
    {
        // Update the bounds of the particle spawner
        SetBounds();
    }

    private void SetBounds()
    {
        // Get active camera position, size, and aspect ratio
        Vector3 activeCameraPosition = cameraManager.GetActiveCamera.transform.position;
        float activeCameraOrthoSize = cameraManager.GetActiveCamera.m_Lens.OrthographicSize;
        float aspectRatio = cameraManager.GetActiveCamera.m_Lens.Aspect;

        // Get the min and max spawn positions for the particles
        Vector3 minPosition = CalculateMinPosition(activeCameraPosition, activeCameraOrthoSize, aspectRatio);
        Vector3 maxPosition = CalculateMaxPosition(activeCameraPosition, activeCameraOrthoSize, aspectRatio);

        if (ambientDustFG != null && ambientDustMG != null)
        {
            // Send information to VFX graphs
            ambientDustFG.SetVector3("Min Position", minPosition);
            ambientDustFG.SetVector3("Max Position", maxPosition);
            ambientDustMG.SetVector3("Min Position", minPosition);
            ambientDustMG.SetVector3("Max Position", maxPosition);
        }

        //if (ambientLeavesFG != null && ambientLeavesMG != null)
        //{
        //    // Update min and max positions Y values before sending them to the leaves so that the leaves always fall from above
        //    maxPosition.y = minPosition.y;
        //    minPosition.y -= 2.0f;

        //    // Update min and max positions X values before sending them to the leaves so that the leaves start falling earlier
        //    //maxPosition.x += maxPosition.x;
        //    //minPosition.x -= minPosition.x;
        //    ambientLeavesFG.SetVector3("Min Position", minPosition);
        //    ambientLeavesFG.SetVector3("Max Position", maxPosition);
        //    ambientLeavesMG.SetVector3("Min Position", minPosition);
        //    ambientLeavesMG.SetVector3("Max Position", maxPosition);
        //} 
    }

    /// <summary>
    /// Calculates the minimum position to spawn particles (top left of camera)
    /// </summary>
    private Vector3 CalculateMinPosition(Vector3 cameraPosition, float cameraOrthoSize, float aspectRatio)
    {
        Vector3 minPosition = new Vector3();

        // Calculate minimum position by getting the left most X and up most Y
        minPosition.x = cameraPosition.x - (aspectRatio * (cameraOrthoSize * 2));
        minPosition.y = cameraPosition.y + (cameraOrthoSize * 2);

        // Return calculate max position
        return minPosition;
    }

    /// <summary>
    /// Calculates the maximum position to spawn particles (bottom right of camera)
    /// </summary>
    private Vector3 CalculateMaxPosition(Vector3 cameraPosition, float cameraOrthoSize, float aspectRatio)
    {
        Vector3 maxPosition = new Vector3();

        // Calculate maximum position by getting the right most X and lower most Y
        maxPosition.x = cameraPosition.x + (aspectRatio * (cameraOrthoSize * 2));
        maxPosition.y = cameraPosition.y - (cameraOrthoSize * 2);

        // Return calculate max position
        return maxPosition;
    }
}
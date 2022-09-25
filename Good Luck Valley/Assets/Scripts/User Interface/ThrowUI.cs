using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowUI : MonoBehaviour
{
    public int segments;

    public float width;

    LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        width = 1f;

        segments = 6;

        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.positionCount = segments + 1;
        lineRenderer.startWidth = width;
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = lineRenderer.startColor;
        lineRenderer.useWorldSpace = true;
    }

    public void PlotTrajectory(Vector2 playerPos, Vector2 forceDirection)
    {
        const float g = 1f;
        float timestep = .2f;
        float tt = 0f;  //elaplse time of the virtual flight
        float x;
        float y;
        float z = 0f;  //since we're in 2d

        lineRenderer.SetPosition(0, playerPos);

        for (int i = 1; i < (segments + 1); i++)
        {
            tt += timestep;  //total elapsed time
            x = playerPos.x + forceDirection.x * (tt);  //note that horizontal velocity component is not affected by gravity
            y = playerPos.y + forceDirection.y * (tt) - 0.5f * g * (tt) * (tt);

            lineRenderer.SetPosition(i, new Vector3(x, y, z));
        }
    }
}

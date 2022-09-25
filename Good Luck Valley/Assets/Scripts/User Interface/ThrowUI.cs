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

        segments = 150;  //just found by trial and arrow/error, since we don't know analytically when it will hit the ground (unless the ground happens to be flat) 

        lineRenderer = gameObject.GetComponent<LineRenderer>();
        lineRenderer.positionCount = (segments + 1);
        lineRenderer.startWidth = width;
        lineRenderer.startColor = Color.black;
        lineRenderer.endColor = Color.black;
        lineRenderer.useWorldSpace = true;
    }

    public void PlotTrajectory(Vector2 initPos, Vector2 initVel, int offset, bool facingRight)
    {
        const float g = 9.8f;
        float timeStep = .2f;
        float tT = 0f;  //elaplse time of the virtual flight
        float x;
        float y;        

        switch (facingRight)
        {
            case true:
                if (initPos.x + initVel.x < initPos.x)
                {
                    initVel = Vector2.zero;
                }
                initPos = new Vector2(initPos.x + offset, initPos.y);
                break;

            case false:
                if (initPos.x - initVel.x < initPos.x)
                {
                    initVel = Vector2.zero;
                }
                initPos = new Vector2(initPos.x - offset, initPos.y);
                break;
        }

        lineRenderer.SetPosition(0, initPos);
        for (int i = 1; i < (segments + 1); i++)
        {
            tT += timeStep;  //total elapsed time
            x = initPos.x + initVel.x * (tT);  //note that horizontal velocity component is not affected by gravity
            y = initPos.y + initVel.y * (tT) - 0.5f * g * (tT) * (tT);

            lineRenderer.SetPosition(i, new Vector3(x, y));
        }

    }
}

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
        // Width of the line
        width = 0.8f;

        // Number of segments for the trajectory line
        segments = 30;  

        // Gets the LineRenderer component from the lineRenderer game object applied in inspector
        lineRenderer = gameObject.GetComponent<LineRenderer>();

        // Sets the number of segmens in the lineRenderer using segments field
        lineRenderer.positionCount = (segments + 1);

        // Sets the with in the lineRenderer using width field
        lineRenderer.startWidth = width;

        // Tells the lineRenderer to use worldspace for defining segmentsx`
        lineRenderer.useWorldSpace = true;

        lineRenderer.sortingLayerName = "UI";
    }

    /// <summary>
    /// Plots the trajeectory of the mushroom during aiming using the pleyer's position, 
    ///     launch force on the mushroom, the offset for spawning, and whether the 
    ///     player is facing right or left
    /// </summary>
    /// <param name="playerPos"> The position of the player AKA starting position for
    ///                             the lineRenderer</param>
    /// <param name="launchForce"> The force the mushroom is being launched at so the
    ///                              lineRenderer can simulate a throw when plotting</param>
    /// <param name="offset"> The offset used when spawning mushrooms</param>
    /// <param name="facingRight"> Whether the player is facing left or right</param>
    public void PlotTrajectory(Vector2 playerPos, Vector2 launchForce, int offset, bool facingRight)
    {
        // Gravity acting on the shroom when it is being thrown
        const float g = 9.8f;

        // Determines how trajectoy line will be
        float timeStep = .2f;

        // Total time that has passed since the lineRenderer started rendering
        float tT = 0f; 

        // X and Y for each segment
        float x;
        float y;

        // Checks whether the player is facing right or left
        switch (facingRight)
        {
            case true:
                // If they are facing right the trajectory line cannot go past the left 
                //    side of the player 
                if (playerPos.x + launchForce.x < playerPos.x)
                {
                    // Sets launchForce to zero to 'stop' the renderer
                    launchForce = Vector2.zero;
                }
                // Sets starting position for line to match the location the shrooms are
                //      spawned from with the offset
                playerPos = new Vector2(playerPos.x + offset, playerPos.y);
                break;

            case false:
                // If the player is facing left the trajectory line cannot go past
                //      the right side of the player
                if (playerPos.x - launchForce.x < playerPos.x)
                {
                    // Sets launchForce to zero to 'stop' the renderer
                    launchForce = Vector2.zero;
                }
                // Sets starting position for line to match the location the shrooms are
                //      spawned from with the offset
                playerPos = new Vector2(playerPos.x - offset, playerPos.y);
                break;
        }

        // Sets the position for the first segment using player position
        Vector3 start = new Vector3(playerPos.x, playerPos.y);
        lineRenderer.SetPosition(0, start);

        // Runs a loop for rendering each segment in the trajectory
        for (int i = 1; i < (segments + 1); i++)
        {
            // Total time passed
            tT += timeStep;  

            // x position is determined by player x + velocity X multiplied
            //  by the amount of time passed
            x = playerPos.x + launchForce.x * (tT); 

            // y position is determined by player x + velocity x multiplied
            //  by time passed - half of gravity multiplied by twice the time passed
            y = playerPos.y + launchForce.y * (tT) - 0.5f * g * (tT) * (tT);

            // Sets the position for this segment using the x and y generated above
            lineRenderer.SetPosition(i, new Vector3(x, y));
        }
    }
}

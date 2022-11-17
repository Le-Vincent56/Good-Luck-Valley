using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class ThrowUI : MonoBehaviour
{
    [SerializeField] MushroomManager mushMan;

    [Header("UI Details")]
    public int segments;
    public float width;

    public LineRenderer lineRenderer = null;

    Vector3[] lineRendererStartingPoints = null;

    // Start is called before the first frame update
    void Start()
    {
        // Width of the line
        width = 0.2f;

        // Number of segments for the trajectory line
        segments = 300;  

        // Gets the LineRenderer component from the lineRenderer game object applied in inspector
        lineRenderer = gameObject.GetComponent<LineRenderer>();
            
        // Sets the number of segmens in the lineRenderer using segments field
        lineRenderer.positionCount = segments;

        lineRendererStartingPoints = new Vector3[segments];

        // Sets the with in the lineRenderer using width field
        lineRenderer.startWidth = width;


        // Tells the lineRenderer to use worldspace for defining segmentsx`
        lineRenderer.useWorldSpace = true;

        lineRenderer.sortingLayerName = "UI";

        lineRenderer.startColor = Color.red;
        lineRenderer.endColor = Color.red;
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
    public void PlotTrajectory(Vector2 playerPos, Vector2 launchForce, bool facingRight)
    {
        // Sets the position count to be the segment count
        lineRenderer.positionCount = segments;
        
        lineRenderer.material.mainTextureScale = new Vector2(1f, 1f);

        // Gravity acting on the shroom when it is being thrown
        const float g = 9.8f;

        // Determines how trajectoy line will be
        float timeStep = .05f;

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
                    // Turns the player by calling playerMovement's Turn method
                    mushMan.player.GetComponent<PlayerMovement>().Turn();
                }
                // Sets starting position for line to match the location the shrooms are
                //      spawned from with the offset
                break;

            case false:
                // if the player is facing left the trajectory line cannot go past
                //      the right side of the player
                if (playerPos.x - launchForce.x < playerPos.x)
                {
                    // sets launchforce to zero to 'stop' the renderer
                    mushMan.player.GetComponent<PlayerMovement>().Turn();
                }
                // sets starting position for line to match the location the shrooms are
                //      spawned from with the offseta
                break;
        }

        // Sets the position for the first segment using player position
        Vector3 start = new Vector3(playerPos.x, playerPos.y, 0);
        lineRenderer.SetPosition(0, start);

        // Runs a loop for rendering each segment in the trajectory
        for (int i = 1; i < segments; i++)
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
            lineRenderer.SetPosition(i, new Vector3(x, y, 0));
            lineRendererStartingPoints[i] = new Vector3(x, y, 0);
        }

        // Creates collided bool, sets to false,
        //  will be used to determin if we should update the segments or not later on
        bool collided = false;

        // Creates hit info variable for storing information about raycast hit
        RaycastHit2D hitInfo;

        // Create new array for storing the new points we will draw with
        Vector3[] newPoints = null;

        // Loops for each point in the previous frame's array of points
        for (int i = 1; i < lineRendererStartingPoints.Length; i++)
        {
            // Create a mask to sort for only 'ground' tiles (collidable)
            LayerMask mask = LayerMask.GetMask("Ground");

            // Sets hit info to the return value of the linecast method,
            //   using the current point on the line and the next point as the locations to check between
            hitInfo = Physics2D.Linecast(lineRendererStartingPoints[i], lineRendererStartingPoints[i + 1], mask);

            // If hit info isnt null, we create the new points
            if (hitInfo)
            {
                //Debug.Log(hitInfo.point);
                //Debug.Log(playerPos);
                // Initializes new points array to be the current iteratin number + 2
                newPoints = new Vector3[i + 2];

                // Loops through each point in the new points array
                for (int k = 0; k < newPoints.Length; k++)
                {
                    // Sets the values in the new points array to match the values in the prev points array
                    newPoints[k] = lineRendererStartingPoints[k];
                }

                // Sets the last position in the new array to be the location the hit occured
                newPoints[i] = hitInfo.point;

                // Collided is true
                collided = true;

                // Breaks out of the array
                break;
            }
        }

        // If collided is true
        if (collided)
        {
            // Sets segments to be equal to the length of the new array so
            //  the line is drawn properly when the top of the method is called
            segments = newPoints.Length;
        }
        else
        {
            // Otherwise, segments is set back to its original value of 30
            segments = 300;
        }
    }

    /// <summary>
    /// Removes the line
    /// </summary>
    public void DeleteLine()
    {
        lineRenderer.positionCount = 0;
    }
}

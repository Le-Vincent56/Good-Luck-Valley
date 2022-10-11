using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedPlatform : MonoBehaviour
{
    [SerializeField] int requiredWeight;
    [SerializeField] float maxDis;
    float initialPosition;
    bool isMoving;


    void Start()
    {
        initialPosition = transform.position.y;
        isMoving = false;
    }

    void Update()
    {
        if (isMoving == true)
        {
            transform.position = new Vector3 (transform.position.x, transform.position.y - 1f, 0); // Updates platforms postions to move it down.

            if (transform.position.y == initialPosition - maxDis) // Reached its max distance platform can move.
            {
                isMoving = false; // Stop moving.
            }
        }
    }

    /// <summary>
    /// Checks if the required amount of shroom to move the platform is on it.
    /// </summary>
    /// <param name="shrooms"></param>
    void CheckWeight(int shrooms)
    {
        if (requiredWeight == shrooms)
        {
            isMoving = true;
        }
    }

}

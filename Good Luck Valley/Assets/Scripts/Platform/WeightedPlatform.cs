using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeightedPlatform : MonoBehaviour
{
    [Header ("Movement")]
    [SerializeField] Vector2 direction;
    [SerializeField] float speed;
    [SerializeField] float maxDistance;
    [SerializeField] bool isMoving;

    Vector2 initialPosition;
    Vector2 platformPosition;
    Vector2 moveDirection;
    Vector2 velocity;

    void Start()
    {
        initialPosition = transform.position;
        isMoving = false;
        platformPosition = new Vector2(transform.position.x, transform.position.y);
    }

    void Update()
    {
        if (isMoving == true)
        {
            // What direction platfrom will move in
            moveDirection = direction;

            // Calculating how much time it will take platfrom to travel to new position
            velocity = moveDirection * speed * Time.deltaTime;

            // Telling platfrom how to move
            platformPosition += velocity;

           // Setting the platfroms original position to new position (moving platfrom).
            transform.position = platformPosition;

            if (Mathf.Abs((int)platformPosition.y) == Mathf.Abs((int)initialPosition.y - maxDistance)) // Platfrom has reached the max distance it can move
            {
                isMoving = false; // Stop moving
            }
        }
    }

    /// <summary>
    /// Checks if the required amount of shroom to move the platform is on it
    /// </summary>
    /// <param name="shrooms"></param>
    public void CheckWeight()
    {
        isMoving = true;
    }
            isMoving = true;
        }
    }

}

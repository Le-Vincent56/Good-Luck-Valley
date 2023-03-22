using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslatablePlatform : MoveablePlatform
{
    [Header("Movement")]
    [SerializeField] Vector3 direction;
    [SerializeField] Vector3 maxDistance;
    [SerializeField] float translateSpeed;

    [SerializeField] Vector3 initialPosition;
    Vector3 platformPosition;
    Vector3 moveDirection;
    Vector3 velocity;

    void Start()
    {
        initialPosition = transform.position;
        platformPosition = new Vector3(transform.position.x, transform.position.y, 0);    
    }

    public override void Move()
    {
        // What direction platfrom will move in
        moveDirection = direction;

        // Calculating how much time it will take platfrom to travel to new position
        velocity = moveDirection * translateSpeed * Time.deltaTime;

        // Telling platfrom how to move
        platformPosition += velocity;

        // Setting the platfroms original position to new position (moving platfrom).
        transform.position = platformPosition;

        // Move shroom with platform
        foreach (GameObject shroom in stuckShrooms)
        {
            shroom.transform.position += velocity;

        }

        if (Mathf.Abs((int)platformPosition.y) == Mathf.Abs((int)initialPosition.y - maxDistance.y)) // Platfrom has reached the max distance it can move
        {
            isTriggered = false; // Stop moving
        }
    }
}     
    



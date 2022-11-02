using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslatablePlatform : MoveablePlatform
{
    [Header("Movement")]
    [SerializeField] Vector2 direction;
    [SerializeField] Vector2 maxDistance;
    [SerializeField] float translateSpeed;

    [SerializeField] Vector2 initialPosition;
    Vector2 platformPosition;
    Vector2 moveDirection;
    Vector2 velocity;

    void Start()
    {
        initialPosition = transform.position;
        platformPosition = new Vector2(transform.position.x, transform.position.y);
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

        if (Mathf.Abs((int)platformPosition.y) == Mathf.Abs((int)initialPosition.y - maxDistance.y)) // Platfrom has reached the max distance it can move
        {
            isTriggered = false; // Stop moving
        }
    }
}     
    



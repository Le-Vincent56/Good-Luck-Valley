using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class BouncingEffect : MonoBehaviour
{
    #region REFERENCES
    [SerializeField] private MovementScriptableObj movementEvent;
    [SerializeField] private MushroomScriptableObj mushroomEvent;
    [SerializeField] private DisableScriptableObj disableEvent;
    #endregion

    #region FIELDS
    [Header("Bounce Variables")]
    [SerializeField] private bool canBounce;
    [SerializeField] private bool onCooldown = false;
    [SerializeField] float bounceClampMin = 0.4f;
    [SerializeField] float bounceClampMax = 0.6f;
    [SerializeField] private Vector3 showForce;
    [SerializeField] private float rotateAngle;
    #endregion

    #region PROPERTIES
    public bool CanBounce { get { return canBounce; } set { canBounce = value; } }
    #endregion

    /// <summary>
    /// Checks collisions, specifically for wall jumps
    /// </summary>
    /// <param name="collision">The Collision2D triggering the collision</param>
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collision is a wall jump object
        if (collision.gameObject.name == "WallJump")
        {
            // Gets the contact points for the collision object
            ContactPoint2D[] contacts = collision.contacts;

            // Gets an initial lowest point 
            Vector2 lowestPoint = contacts[0].point;

            // Loops through the contacts length
            for (int i = 1; i < contacts.Length; i++) 
            { 
                // Checks if the contact point is less than the current lowest point
                if (contacts[i].point.y < lowestPoint.y)
                {
                    // Sets the lowest point to the current contact point
                    lowestPoint = contacts[i].point;
                }
            }

            //if (lowestPoint.y < transform.position.y)
            //{
            //}
            // Sets touching wall to true and assigns the mushroom position to the lowest point
            movementEvent.SetIsTouchingWall(true);
            movementEvent.SetMushroomPosition(lowestPoint);
            movementEvent.SetWallCollisionPoint(contacts[0].point);
        }
    }

    /// <summary>
    /// Updates the shroom position and whether we are touching a wall 
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionStay2D(Collision2D collision)
    {
        // Checks if the game object is a walljump object
        if (collision.gameObject.name == "WallJump")
        {
            // Gets the contact points
            ContactPoint2D[] contacts = collision.contacts;

            // Gets an initial lowest point 
            Vector2 lowestPoint = contacts[0].point;

            // Loops through the contacts length
            for (int i = 0; i < contacts.Length; i++)
            {
                // Checks if the contact point is less than the current lowest point
                if (contacts[i].point.y < lowestPoint.y)
                {
                    // Sets the lowest point to the current contact point
                    lowestPoint = contacts[i].point;
                }
            }

            //if (lowestPoint.y < transform.position.y)
            //{
            //}
            // Sets touching wall to true and assigns the mushroom position to the lowest point and the wall collision point
            movementEvent.SetIsTouchingWall(true);
            movementEvent.SetMushroomPosition(lowestPoint);
            movementEvent.SetWallCollisionPoint(contacts[0].point);
            //mushroomEvent.SetTouchingShroom(true);
            //mushroomEvent.TouchingShroom();
        }
    }

    /// <summary>
    /// Checks for when the object is not touching the Mushroom
    /// </summary>
    /// <param name="collision">The Collision2D checking for an exit</param>
    void OnCollisionExit2D(Collision2D collision)
    {
        // Check if the collision is a walljump object
        if (collision.gameObject.name == "WallJump")
        {
            // Sets touching wall to false and the mushroom position to off screen
            movementEvent.SetIsTouchingWall(false);
            movementEvent.SetMushroomPosition(new Vector3(-1000, -1000, -1000));
        }
    }

    /// <summary>
    /// Cgec
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if we are not touching a shroom already and that the trigger we are touching is a mushroom
        if (!mushroomEvent.IsTouchingShroom && collision.gameObject.CompareTag("Mushroom"))
        {
            // Sets the velocity of the playe to 0
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            // Gets the mushroom info form the collision game object
            Shroom shroomToBounce = collision.gameObject.GetComponent<Shroom>();

            // Check to see if there is a shroom component
            if (shroomToBounce != null)
            {
                // Remove tutorial message
                if (mushroomEvent.GetFirstBounce())
                {
                    mushroomEvent.HideBounceMessage();
                }

                // Set touching shroom to true if colliding with the mushroom
                mushroomEvent.SetTouchingShroom(true);
                mushroomEvent.TouchingShroom();
                movementEvent.SetIsGrounded(false);
                movementEvent.SetIsBouncing(true);
                shroomToBounce.Bounce();
            }

            // Trigger shroom bounce animation, if it exists
            Animator shroomAnimator = collision.gameObject.GetComponent<Animator>();
            if(shroomAnimator != null)
            {
                shroomAnimator.SetTrigger("Bouncing");
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if the collider is a mushroom
        if (collision.gameObject.CompareTag("Mushroom"))
        {
            // If exiting, set touchingShroom to false
            mushroomEvent.SetTouchingShroom(false);
            mushroomEvent.TouchingShroom();
        }
    }
}

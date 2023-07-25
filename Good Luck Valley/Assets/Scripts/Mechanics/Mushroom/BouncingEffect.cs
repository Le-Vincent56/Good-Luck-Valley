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
    /// Checks Collisions, specifically with Mushrooms, and sees if a bounce can trigger
    /// </summary>
    /// <param name="collision">The Collision2D triggering the collision</param>
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "WallJump")
        {
            ContactPoint2D[] contacts = collision.contacts;

            Vector2 lowestPoint = contacts[0].point;

            for (int i = 0; i < contacts.Length; i++) 
            { 
                if (contacts[i].point.y < lowestPoint.y)
                {
                    lowestPoint = contacts[i].point;
                }
            }
            movementEvent.SetIsTouchingWall(true);
            movementEvent.SetMushroomPosition(lowestPoint);
            //mushroomEvent.SetTouchingShroom(true);
            //mushroomEvent.TouchingShroom();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!mushroomEvent.IsTouchingShroom && collision.gameObject.tag == "Mushroom")
        {
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            Shroom shroomToBounce = collision.gameObject.GetComponent<Shroom>();

            // Check to see if there is a shroom component
            if (shroomToBounce != null)
            {
                // Cuts momentum before applying bounce 

                // Set touching shroom to true if colliding with the mushroom
                mushroomEvent.SetTouchingShroom(true);
                mushroomEvent.TouchingShroom();
                movementEvent.SetIsGrounded(false);
                shroomToBounce.Bounce();
            }

            Animator shroomAnimator = collision.gameObject.GetComponent<Animator>();

            if(shroomAnimator != null)
            {
                shroomAnimator.SetTrigger("Bouncing");
            }
        }
    }

    /// <summary>
    /// Checks for when the object is not touching the Mushroom
    /// </summary>
    /// <param name="collision">The Collision2D checking for an exit</param>
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.name == "WallJump")
        {
            movementEvent.SetIsTouchingWall(false);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.name == "WallJump")
        {
            ContactPoint2D[] contacts = collision.contacts;

            Vector2 lowestPoint = contacts[0].point;

            for (int i = 0; i < contacts.Length; i++)
            {
                if (contacts[i].point.y < lowestPoint.y)
                {
                    lowestPoint = contacts[i].point;
                }
            }
            movementEvent.SetIsTouchingWall(true);
            movementEvent.SetMushroomPosition(lowestPoint);
            movementEvent.SetWallCollisionPoint(contacts[0].point);
            //mushroomEvent.SetTouchingShroom(true);
            //mushroomEvent.TouchingShroom();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Check if the collider is a mushroom
        if (collision.gameObject.tag.Equals("Mushroom"))
        {
            // If exiting, set touchingShroom to false
            mushroomEvent.SetTouchingShroom(false);
            mushroomEvent.TouchingShroom();
        }
    }
}

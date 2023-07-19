using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
    [SerializeField] private float angleToSubtract;
    [SerializeField] private float bounceForce;
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
        if (!mushroomEvent.IsTouchingShroom)
        {
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            Shroom shroomToBounce = collision.gameObject.GetComponent<Shroom>();
            if (shroomToBounce != null)
            {
                // Cuts momentum before applying bounce

                shroomToBounce.Bounce();
            }

            if (collision.gameObject.tag.Equals("Mushroom"))
            {
                // Set touching shroom to true if colliding with the mushroom
                mushroomEvent.SetTouchingShroom(true);
                mushroomEvent.TouchingShroom();
            }
        }

        if (collision.gameObject.name == "WallJump")
        {
            GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            ContactPoint2D contact = collision.GetContact(0);

            if (contact.point.x > transform.position.x)
            {
                Debug.Log("Rotate Angle: " + rotateAngle);
                rotateAngle = -180;
            }
            else
            {
                rotateAngle = 0;
            }
            SetBounceForce();
            movementEvent.SetIsTouchingWall(true);
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
        // Check if the collider is a mushroom
        if(collision.gameObject.tag.Equals("Mushroom"))
        {
            // If exiting, set touchingShroom to false
            mushroomEvent.SetTouchingShroom(false);
            mushroomEvent.TouchingShroom();
        }

        if (collision.gameObject.name == "WallJump")
        {
            movementEvent.SetIsTouchingWall(false);
        }
    }
    public void SetBounceForce()
    {
        // Calculate the force to apply
        Quaternion rotation = Quaternion.AngleAxis(rotateAngle - angleToSubtract, Vector3.forward);
        Vector3 direction = rotation * Vector2.up;

        // If the rotate angle is 0, then invert y direction to send upwards
        if (rotateAngle >= 0)
        {
            direction.y *= -1;
        }


        Vector3 forceToApply = direction * bounceForce;
        showForce = forceToApply;

        movementEvent.SetBounceForce(forceToApply);

        // Disable input
        //disableEvent.SetInputCooldown(0.05f);
        //disableEvent.StopInput();
        //
        //Debug.Log("Bounce: " + forceToApply);
        //movementEvent.Bounce(forceToApply, ForceMode2D.Impulse);
    }
}

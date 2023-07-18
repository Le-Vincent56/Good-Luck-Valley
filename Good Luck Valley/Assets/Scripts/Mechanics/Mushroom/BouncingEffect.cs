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
            Shroom shroomToBounce = collision.gameObject.GetComponent<Shroom>();
            if (shroomToBounce != null)
            {
                // Cuts momentum before applying bounce
                GetComponent<Rigidbody2D>().velocity = Vector3.zero;

                shroomToBounce.Bounce();
            }

            if (collision.gameObject.tag.Equals("Mushroom"))
            {
                // Set touching shroom to true if colliding with the mushroom
                mushroomEvent.SetTouchingShroom(true);
                mushroomEvent.TouchingShroom();
            }
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
    }
}

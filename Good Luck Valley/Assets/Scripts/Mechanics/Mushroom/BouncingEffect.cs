using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.EventSystems;

public class BouncingEffect : MonoBehaviour
{
    #region REFERENCES
    private Rigidbody2D RB;
    #endregion

    #region FIELDS
    #region TUTORIAL
    [SerializeField] Tutorial tutorialManager;
    private bool firstBounce = true;
    #endregion

    [Header("Bounce Variables")]
    [SerializeField] private bool canBounce;
    [SerializeField] private bool onCooldown = false;
    [SerializeField] float bounceForce = 15f;
    [SerializeField] float bounceClampMin = 0.4f;
    [SerializeField] float bounceClampMax = 0.6f;
    [SerializeField] private Vector2 lastVelocity;
    private float cooldown = 0.1f;
    private Vector2 direction;
    #endregion

    #region PROPERTIES
    public bool CanBounce { get { return canBounce; } set { canBounce = value; } }
    #endregion

    void Start()
    {
        // Get components
        RB = GetComponent<Rigidbody2D>();

        // tutorialManager is added through the Inspector

        cooldown = 0.1f;
    }

    void Update()
    {
        // Create a bounce buffer so that if the player immediately hits a slope
        // or wall, it does not end the bouncing

        if (onCooldown)
        {
            cooldown -= Time.deltaTime;
        }

        if(cooldown <= 0)
        {
            onCooldown = false;
            cooldown = 0.1f;
        }

        // Get last velocity
        lastVelocity = RB.velocity;
    }

    /// <summary>
    /// Checks Collisions, specifically with Mushrooms, and sees if a bounce can trigger
    /// </summary>
    /// <param name="collision">The Collision2D triggering the collision</param>
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Mushroom") && collision.collider is CircleCollider2D)
        {
            // Check if colliding with a mushroom
            if (!onCooldown)
            {
                RB.velocity = new Vector2(Mathf.Clamp(RB.velocity.x, bounceClampMin, bounceClampMax), RB.velocity.y);

                // If there is a tutorialManager, and firstBounce is true,
                // don't show bounce tutorial text and set firstBounce to false
                if (tutorialManager != null && firstBounce)
                {
                    tutorialManager.ShowingBounceText = false;
                    firstBounce = false;
                }

                // Set the MushroomInfo to bouncing
                collision.gameObject.GetComponent<MushroomInfo>().Bouncing = true;
                collision.gameObject.GetComponent<MushroomInfo>().BouncingTimer = 1f;

                // Set the direction
                Quaternion rotation = Quaternion.AngleAxis(collision.gameObject.GetComponent<MushroomInfo>().RotateAngle - 90, Vector3.forward);
                direction = rotation * Vector2.up;

                // Get a number between 0 and 0.5 depending on the angle (besides some edge cases)
                float rotationDegrees = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                float roundedPercentage = (rotationDegrees % 90) / 90;
                float precisePercentage = Mathf.Abs(0f - roundedPercentage);

                // Check for edge cases - when it might be around 0.98
                if(precisePercentage > 0.5f && precisePercentage < 1.0f)
                {
                    precisePercentage = Mathf.Abs(precisePercentage - 1);
                }

                // Find the additional force
                float additionalForce = bounceForce * (precisePercentage / 3);

                // Apply bounce
                RB.AddForce(direction * (bounceForce + additionalForce), ForceMode2D.Impulse);
                onCooldown = true;

                // If additional force is greater than 0.1, that means you're likely not at an angle, so apply a movement cooldown so the bounce feels most impactful
                if (additionalForce > 0.1f)
                {
                    EventManager.TriggerEvent("StopInput", 0.05f);
                }

                // Trigger events
                EventManager.TriggerEvent("Bounce", true);
                EventManager.TriggerEvent("Bounce");
            }
        } else if(collision.gameObject.tag.Equals("Mushroom"))
        {
            // Set touching shroom to true if colliding with the mushroom
            EventManager.TriggerEvent("TouchingShroom", true);
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
            EventManager.TriggerEvent("TouchingShroom", false);
        }
    }
}

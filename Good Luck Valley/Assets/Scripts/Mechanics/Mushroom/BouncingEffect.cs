using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class BouncingEffect : MonoBehaviour
{
    #region REFERENCES
    private MushroomInfo mushroomInfo;
    private Rigidbody2D RB;
    private BoxCollider2D playerCollider;
    #endregion

    #region FIELDS
    #region TUTORIAL
    [SerializeField] Tutorial tutorialManager;
    private bool firstBounce = true;
    #endregion

    [Header("Bounce Variables")]
    [SerializeField] float bounceForce = 15f;
    [SerializeField] private bool canBounce;
    [SerializeField] private bool onCooldown = false;
    private float cooldown = 0.1f;
    [SerializeField] float bounceClampMin = 0.4f;
    [SerializeField] float bounceClampMax = 0.6f;
    private float speed;
    private Vector2 direction;
    [SerializeField] private Vector2 lastVelocity;
    #endregion

    #region PROPERTIES
    public bool CanBounce { get { return canBounce; } set { canBounce = value; } }
    #endregion

    void Start()
    {
        // Get components
        mushroomInfo = GetComponent<MushroomInfo>();
        RB = GetComponent<Rigidbody2D>();
        playerCollider = GetComponent<BoxCollider2D>();

        // PlayerData is added through the Inspector
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

                // Get the calculated speed based on last Velocity
                speed = lastVelocity.magnitude;

                // Set the direction
                Quaternion rotation = Quaternion.AngleAxis(collision.gameObject.GetComponent<MushroomInfo>().RotateAngle - 90, Vector3.forward);
                direction = rotation * Vector2.up;

                // TODO ROTATION-PERCENTAGE
                // Maximum additional force percentage on
                float rotationDegrees = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                Debug.Log("Rotation Degrees: " + rotationDegrees);

                Debug.Log("Mod: " + rotationDegrees % 90);

                float roundedPercentage = (rotationDegrees % 90) / 90;

                float precisePercentage = Mathf.Abs(0f - roundedPercentage);

                if(precisePercentage > 0.5f && precisePercentage < 1.0f)
                {
                    precisePercentage = Mathf.Abs(precisePercentage - 1);
                }

                float additionalForce = bounceForce * (precisePercentage / 3);

                RB.AddForce(direction * (bounceForce + additionalForce), ForceMode2D.Impulse);


                onCooldown = true;

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

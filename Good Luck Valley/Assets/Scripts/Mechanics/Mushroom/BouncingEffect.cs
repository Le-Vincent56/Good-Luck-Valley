using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class BouncingEffect : MonoBehaviour
{
    #region REFERENCES
    private MushroomInfo mushroomInfo;
    private Rigidbody2D RB;
    private PlayerMovement playerMovement;
    private BoxCollider2D playerCollider;
    [SerializeField] PlayerData playerData;
    #endregion

    #region FIELDS
    #region TUTORIAL
    [SerializeField] Tutorial tutorialManager;
    private bool firstBounce = true;
    #endregion

    [Header("Bounce Variables")]
    [SerializeField] float bounceForce = 15f;
    [SerializeField] private bool bouncing;
    [SerializeField] private bool canBounce;
    [SerializeField] private bool onCooldown = false;
    private float bounceBuffer = 0.1f;
    private float cooldown = 0.1f;
    [SerializeField] float movementCooldown = 0.5f;
    [SerializeField] bool touchingShroom = false;
    [SerializeField] float bounceClampMin = 0.4f;
    [SerializeField] float bounceClampMax = 0.6f;

    private float speed;
    private Vector2 direction;
    [SerializeField] private Vector2 lastVelocity;
    #endregion

    #region PROPERTIES
    public bool Bouncing { get { return bouncing; } set { bouncing = value; } }
    public bool CanBounce { get { return canBounce; } set { canBounce = value; } }
    public float BounceBuffer { get { return bounceBuffer; } set { bounceBuffer = value; } }
    public bool TouchingShroom { get { return touchingShroom; } set { touchingShroom = value; } }
    #endregion

    void Start()
    {
        // Get components
        mushroomInfo = GetComponent<MushroomInfo>();
        RB = GetComponent<Rigidbody2D>();
        playerMovement = GetComponent<PlayerMovement>();
        playerCollider = GetComponent<BoxCollider2D>();

        // PlayerData is added through the Inspector
        // tutorialManager is added through the Inspector

        cooldown = 0.1f;

    }

    void Update()
    {
        // Create a bounce buffer so that if the player immediately hits a slope
        // or wall, it does not end the bouncing
        if(bounceBuffer > 0 && bouncing)
        {
            bounceBuffer -= Time.deltaTime;
        }

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

                // Disable movement for a little bit
                playerMovement.DisableInputTimer = movementCooldown;

                // If there is a tutorialManager, and firstBounece is true,
                // don't show bounce tutorial text and set firstBounce to false
                if (tutorialManager != null && firstBounce)
                {
                    tutorialManager.ShowingBounceText = false;
                    firstBounce = false;
                }

                // Set bouncing to true
                bouncing = true;
                bounceBuffer = 0.1f;

                // Reset landed timer
                playerMovement.LandedTimer = 0.2f;

                // Set the MushroomInfo to bouncing
                collision.gameObject.GetComponent<MushroomInfo>().Bouncing = true;
                collision.gameObject.GetComponent<MushroomInfo>().BouncingTimer = 1f;

                // Get the calculated speed based on last Velocity
                speed = lastVelocity.magnitude;

                // Set the direction
                Quaternion rotation = Quaternion.AngleAxis(collision.gameObject.GetComponent<MushroomInfo>().RotateAngle - 90, Vector3.forward);
                direction = rotation * Vector2.up;

                //RB.AddForce(Mathf.Max(speed, minSpeed) * direction, ForceMode2D.Impulse);
                RB.AddForce(direction * bounceForce, ForceMode2D.Impulse);
                onCooldown = true;

                // Trigger events
                EventManager.TriggerEvent("Bouncing", true);
            }
        } else if(collision.gameObject.tag.Equals("Mushroom"))
        {
            // Set touching shroom to true if colliding with the mushroom
            touchingShroom = true;
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
            touchingShroom = false;
        }
    }
}

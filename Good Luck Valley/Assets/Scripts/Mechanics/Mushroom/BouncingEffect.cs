using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingEffect : MonoBehaviour
{
    #region REFERENCES
    private Rigidbody2D RB;
    private Animator animator;
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
    [SerializeField] float minSpeed = 100f; // 140 original minimumSpeed
    [SerializeField] private bool bouncing;
    [SerializeField] private bool canBounce;
    [SerializeField] private bool onCooldown = false;
    private float bounceBuffer = 0.1f;
    private float cooldown = 0.1f;

    private float speed;
    private Vector2 direction;
    [SerializeField] private Vector2 lastVelocity;
    #endregion

    #region PROPERTIES
    public bool Bouncing { get { return bouncing; } set { bouncing = value; } }
    public bool CanBounce { get { return canBounce; } set { canBounce = value; } }
    public float BounceBuffer { get { return bounceBuffer; } set { bounceBuffer = value; } }
    #endregion

    void Start()
    {
        // Get components
        RB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
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
        // Check if colliding with a mushroom
        if (collision.gameObject.tag.Equals("Mushroom") && RB.velocity.x < 0.1f && !onCooldown)
        {
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
            animator.SetTrigger("Bouncing");
            collision.gameObject.GetComponent<MushroomInfo>().Bouncing = true;
            collision.gameObject.GetComponent<MushroomInfo>().BouncingTimer = 1f;

            // Set the direction
            direction = Vector2.up;

            RB.AddForce(direction * minSpeed, ForceMode2D.Impulse);
            onCooldown = true;
        }
        else if (collision.gameObject.tag.Equals("Mushroom") && canBounce && !onCooldown)
        {
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
            animator.SetTrigger("Bouncing");
            collision.gameObject.GetComponent<MushroomInfo>().Bouncing = true;
            collision.gameObject.GetComponent<MushroomInfo>().BouncingTimer = 1f;

            // Get the calculated speed based on last Velocity
            speed = lastVelocity.magnitude;

            //Vector2 velDirection = lastVelocity.normalized;
            //if (speed <= 0.01f)
            //{
            //    velDirection = Vector2.up;
            //    speed = 5f;
            //}

            // Set the direction
            direction = Vector2.Reflect(lastVelocity.normalized, collision.GetContact(0).normal);

            RB.AddForce(direction * Mathf.Max(speed, minSpeed), ForceMode2D.Impulse);
            onCooldown = true;
        }
    }
}

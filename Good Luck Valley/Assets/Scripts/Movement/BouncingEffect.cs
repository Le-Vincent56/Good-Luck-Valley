using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingEffect : MonoBehaviour
{
    #region REFERENCES
    private Rigidbody2D RB;
    private Animator animator;
    private PlayerMovement playerMovement;
    [SerializeField] PlayerData playerData;
    #endregion

    #region FIELDS
    #region TUTORIAL
    [SerializeField] Tutorial tutorialManager;
    private bool firstBounce = true;
    #endregion

    [Header("Bounce Variables")]
    [SerializeField] float minSpeed = 100f; // 140 original minimumSpeed
    private bool bouncing;
    private bool canBounce;

    private float speed;
    private Vector2 direction;
    private Vector2 lastVelocity;
    #endregion

    #region PROPERTIES
    public bool Bouncing { get { return bouncing; } set { bouncing = value; } }
    public bool CanBounce { get { return canBounce; } set { canBounce = value; } }
    #endregion

    void Start()
    {
        // Get components
        RB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();

        // PlayerData is added through the Inspector
        // tutorialManager is added through the Inspector

    }

    void Update()
    {
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
        if (collision.gameObject.tag.Equals("Mushroom") && canBounce)
        {
            // If there is a tutorialManager, and firstBounece is true,
            // don't show bounce tutorial text and set firstBounce to false
            if(tutorialManager != null && firstBounce)
            {
                tutorialManager.ShowingBounceText = false;
                firstBounce = false;
            }

            // Set bouncing to true
            bouncing = true;

            // Set the MushroomInfo to bouncing
            collision.gameObject.GetComponent<Animator>().SetBool("Bouncing", true);
            collision.gameObject.GetComponent<MushroomInfo>().Bouncing = true;
            collision.gameObject.GetComponent<MushroomInfo>().BouncingTimer = 1f;

            // Get the calculated speed based on last Velocity
            speed = lastVelocity.magnitude;

            // Set the direction
            direction = Vector2.Reflect(lastVelocity.normalized, collision.GetContact(0).normal);
            
            RB.AddForce(direction * Mathf.Max(speed, minSpeed), ForceMode2D.Impulse);
        }
    }
}

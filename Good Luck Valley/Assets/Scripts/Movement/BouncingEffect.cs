using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingEffect : MonoBehaviour
{
    #region FIELDS
    [Header("Components")]
    public Rigidbody2D RB;
    public Animator animator;
    public PlayerMovement playerMovement;

    [Header("Bounce Variables")]
    [SerializeField] float minSpeed = 100f;
    public bool bouncing = false;

    Vector2 lastVelocity;
    #endregion

    void Start()
    {
        // Get components
        RB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        // Get last velocity
        lastVelocity = RB.velocity;

        // Check if play is still bouncing
        if (!playerMovement._isJumpFalling)
        {
            bouncing = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if colliding with a mushroom
        if (collision.gameObject.tag.Equals("Mushroom"))
        {
            // Set bouncing to true
            bouncing = true;

            // Get the calculated speed based on last Velocity
            float speed = lastVelocity.magnitude;

            // Set the direction
            Vector2 direction = Vector2.Reflect(lastVelocity.normalized, collision.GetContact(0).normal);
            RB.velocity = (direction * Mathf.Max(speed, minSpeed));
        }
    }
}

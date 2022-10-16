using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingEffect : MonoBehaviour
{
    #region FIELDS
    public Rigidbody2D RB;
    public Animator animator;
    public PlayerMovement playerMovement;

    Vector2 lastVelocity;
    [SerializeField] float minSpeed = 100f;
    public bool bouncing = false;
    #endregion

    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    void Update()
    {
        lastVelocity = RB.velocity;

        if (!playerMovement._isJumpFalling)
        {
            bouncing = false;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Mushroom"))
        {
            bouncing = true;

            // Get the calculated speed based on last Velocity
            float speed = lastVelocity.magnitude;

            // Set the direction
            Vector2 direction = Vector2.Reflect(lastVelocity.normalized, collision.GetContact(0).normal);
            RB.velocity = (direction * Mathf.Max(speed, minSpeed));
        }
    }
}

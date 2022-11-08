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
    public PlayerData playerData;

    [Header("Bounce Variables")]
    [SerializeField] float minSpeed = 100f; // 140 original minimumSpeed
    public bool bouncing;
    public bool canBounce;

    float speed;
    Vector2 direction;
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
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if colliding with a mushroom
        if (collision.gameObject.tag.Equals("Mushroom") && canBounce)
        {
            // Set bouncing to true
            bouncing = true;

            // Get the calculated speed based on last Velocity
            speed = lastVelocity.magnitude;

            // Set the direction
            direction = Vector2.Reflect(lastVelocity.normalized, collision.GetContact(0).normal);
            
            RB.AddForce(direction * Mathf.Max(speed, minSpeed), ForceMode2D.Impulse);
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingEffect : MonoBehaviour
{
    #region FIELDS
    public Rigidbody2D RB;
    public Animator animator;

    Vector2 lastVelocity;
    [SerializeField] float minimumBounce = 200;
    [SerializeField] float maximumBounce = 205;
    #endregion

    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        lastVelocity = RB.velocity;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Mushroom"))
        {
            Debug.Log("RB Velocity Before Calc: " + RB.velocity);

            // Get the calculated speed based on last Velocity
            float speed = lastVelocity.magnitude;

            Debug.Log("Speed Value: " + speed);

            // Set a minimum "bounce" speed
            if(speed < minimumBounce)
            {
                speed = minimumBounce;
            }

            // Set the direction
            Vector2 direction = Vector2.Reflect(lastVelocity.normalized, collision.GetContact(0).normal);

            Debug.Log("Direction Vector: " + direction);

            RB.AddForce(direction * speed, ForceMode2D.Impulse);

            Debug.Log("RB Velocity After Calc: " + RB.velocity);

            
        }
    }
}

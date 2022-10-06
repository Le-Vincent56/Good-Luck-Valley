using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingEffect : MonoBehaviour
{
    #region FIELDS
    public Rigidbody2D RB;
    public Animator animator;

    Vector3 lastVelocity;
    [SerializeField] float minimumBounce = 100;
    #endregion

    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        lastVelocity = RB.velocity;
        Debug.Log(lastVelocity.magnitude);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag.Equals("Mushroom"))
        {
            // Get the calculated speed based on last Velocity
            float speed = lastVelocity.magnitude;

            // Set a minimum "bounce" speed
            if(lastVelocity.magnitude < minimumBounce)
            {
                speed = minimumBounce;
            }

            // Set the direction
            Vector3 direction = Vector3.Reflect(lastVelocity.normalized, collision.contacts[0].normal);

            // Apply the bounce
            RB.velocity = direction * Mathf.Max(speed, 0f);
        }
    }
}

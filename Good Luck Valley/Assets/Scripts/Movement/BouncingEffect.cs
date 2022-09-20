using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncingEffect : MonoBehaviour
{
    #region FIELDS
    public Rigidbody2D RB;
    public Animator animator;

    Vector3 epicentro;

    [Header("Properties")]
    public float radius = 5.0F;
    public float power = 1000.0F;
    #endregion

    void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        Vector3 explosionPos = transform.position;
        epicentro = explosionPos;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Mushroom"))
        {
            RB.AddExplosionForce(power, epicentro, radius);
            Debug.Log("Player has collided with Bouncer");
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class WallShroom : Shroom
{
    #region FIELDS
    [SerializeField] private float angleToSubtract;
    #endregion

    private void Awake()
    {
        mushMan = GameObject.Find("Mushroom Manager").GetComponent<MushroomManager>();
        shroomCounter = GameObject.Find("MushroomCountUI").GetComponent<ShroomCounter>();
        shroomParticles = GetComponent<ParticleSystem>();
        durationTimer = mushMan.ShroomDuration;
        defaultColor = new Color(168, 168, 168);
        particleTime = durationTimer;
        isShroom = true;
    }

    // Update is called once per frame
    void Update()
    {
        SetCooldown();

        if (bouncing)
        {
            bouncingTimer -= Time.deltaTime;
            if (bouncingTimer <= 0)
            {
                bouncing = false;
                GetComponent<Animator>().SetBool("Bouncing", false);
            }
        }
        Debug.Log("Gen Shroom is shroom?: " + isShroom);
        if (isShroom && mushMan.EnableShroomTimers)
        {
            Debug.Log("Shroom Counter Updating for " + shroomType);
            UpdateShroomCounter();
        }
        else if (mushMan.EnableShroomTimers && mushMan.MushroomLimit == 3)
        {
            Debug.Log("Nothing");
        }
    }

    /// <summary>
    /// Bounce the player
    /// </summary>
    public override void Bounce()
    {
        Debug.Log("Bounce");

        // Calculate the force to apply
        Quaternion rotation = Quaternion.AngleAxis(rotateAngle - angleToSubtract, Vector3.forward);
        Vector3 direction = rotation * Vector2.up;

        // If the rotate angle is 0, then invert y direction to send upwards
        if(rotateAngle >= 0)
        {
            direction.y *= -1;
        }

        Vector3 forceToApply = direction * bounceForce;

        // Disable input
        disableEvent.SetInputCooldown(0.05f);
        disableEvent.StopInput();

        // Apply bounce
        mushroomEvent.SetBounce(true);
        mushroomEvent.Bounce(forceToApply, ForceMode2D.Impulse);
    }

    /// <summary>
    /// Check for collisions
    /// </summary>
    /// <param name="collision">The context of the collider</param>
    public override void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasRotated)
        {
            if (collision.collider is CompositeCollider2D)
            {
                // Set shroom type
                IShroomeable shroomeableTile = collision.gameObject.GetComponent<IShroomeable>();
                if (shroomeableTile != null)
                {
                    shroomType = shroomeableTile.GetType();
                }
                else
                {
                    shroomType = ShroomType.Regular;
                }

                // Rotate and freeze the shroom
                RotateAndFreeze();
            }
        }
    }
}

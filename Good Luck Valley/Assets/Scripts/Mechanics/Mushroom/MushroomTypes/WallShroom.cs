using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class WallShroom : Shroom
{
    #region FIELDS
    [SerializeField] private float angleToSubtract;
    private Vector3 showForce;
    
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
        if (isShroom && mushMan.EnableShroomTimers)
        {
            UpdateShroomCounter();
        }
        else if (mushMan.EnableShroomTimers && mushMan.MushroomLimit == 3)
        {
            Debug.Log("Nothing");
        }

        Debug.DrawRay(transform.position, showForce);
    }

    public override void UpdateShroomCounter()
    {
        if (durationTimer <= 0)
        {
            Destroy(gameObject);
            return;
        }
        // Decreases time from the timer
        durationTimer -= Time.deltaTime;

        if (durationTimer <= (particleTime * 0.5) && playShroomParticle)
        {
            shroomParticles.Play();
            playShroomParticle = false;
        }


        // The percent that should be reducted from the opacity each frame
        //float percentOpacity = Time.deltaTime / mushMan.ShroomDuration;
        if (durationTimer <= .1f)
        {
            float percentOpacity = Time.deltaTime / .1f;

            // Adjust opacity of mushroom and intensity of light based on percentOpacity
            GetComponent<SpriteRenderer>().color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, GetComponent<SpriteRenderer>().color.a - percentOpacity);
            GetComponentInChildren<Light2D>().intensity -= percentOpacity;
        }

        if (mushMan.MushroomLimit == 3)
        {
            if (mushMan.ThrowUnlocked)
            {
                shroomIcon.GetComponent<Image>().fillAmount += (Time.deltaTime / mushMan.ShroomDuration);
            }

            if (shroomIcon.GetComponent<Image>().fillAmount >= 1f)
            {
                shroomIcon.GetComponent<ParticleSystem>().Play();
            }
        }
    }

    /// <summary>
    /// Bounce the player
    /// </summary>
    public override void Bounce()
    {
        // Calculate the force to apply
        Quaternion rotation = Quaternion.AngleAxis(rotateAngle - angleToSubtract, Vector3.forward);
        Vector3 direction = rotation * Vector2.up;

        // If the rotate angle is 0, then invert y direction to send upwards
        if(rotateAngle >= 0)
        {
            direction.y *= -1;
        }

        if (flipRotation)
        {
            direction.x *= -1;
        }

        Vector3 forceToApply = direction * bounceForce;
        showForce = forceToApply;

        // Disable input
        disableEvent.SetInputCooldown(0.15f);
        disableEvent.StopInput();

        // Apply bounce
        if(!bouncing)
        {
            bouncing = true;
        }

        movementEvent.SetIsBounceAnimating(true);
        movementEvent.Bounce(forceToApply, ForceMode2D.Impulse);
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
                Debug.Log("Rotate and Freeze is called");
                RotateAndFreeze(rotation);
            }
        }
    }
}

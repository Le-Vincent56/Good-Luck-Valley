using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Unity.VisualScripting;

public class GeneralShroom : Shroom
{
    #region REFERENCES
    // [SerializeField] Tutorial tutorialManager;
    #endregion

    #region FIELDS
    // private bool firstBounce = true;
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

        // Check if the player is bouncing
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

        if (durationTimer <= (particleTime * 0.5) && playShroomParticle && shroomParticles != null)
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

        // Checks if infinite shrooms is disabled
        if (mushMan.MushroomLimit == 3)
        {
            // Checks if the throw has been unlocked
            if (mushMan.ThrowUnlocked)
            {
                // Increases the fill amount
                shroomIcon.GetComponent<Image>().fillAmount += (Time.deltaTime / mushMan.ShroomDuration);
            }

            // Checks if the fill is full
            if (shroomIcon.GetComponent<Image>().fillAmount >= 1f)
            {
                // Plays the icon particle effect
                shroomIcon.GetComponent<ParticleSystem>().Play();
            }
        }
    }

    /// <summary>
    /// Bounce the player
    /// </summary>
    public override void Bounce()
    {
        // Check if colliding with a mushroom
        if (!onCooldown)
        {
            //// If there is a tutorialManager, and firstBounce is true,
            //// don't show bounce tutorial text and set firstBounce to false
            //if (tutorialManager != null && firstBounce)
            //{
            //    tutorialManager.ShowingBounceText = false;
            //    firstBounce = false;
            //}

            // Set bouncing variables
            // Trigger events
            if (!bouncing)
            {
                bouncing = true;
            }
            bouncingTimer = 1f;

            // Set the direction
            Quaternion rotation = Quaternion.AngleAxis(rotateAngle - 90, Vector3.forward);
            Vector3 direction = rotation * Vector2.up;

            // Get a number between 0 and 0.5 depending on the angle (besides some edge cases)
            float rotationDegrees = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            float roundedPercentage = (rotationDegrees % 90) / 90;
            float precisePercentage = Mathf.Abs(0f - roundedPercentage);

            // Check for edge cases - when it might be around 0.98
            if (precisePercentage > 0.5f && precisePercentage < 1.0f)
            {
                precisePercentage = Mathf.Abs(precisePercentage - 1);
            }

            // Find the additional force
            float additionalForce = bounceForce * (precisePercentage / 4);

            // Apply bounce
            Vector3 forceToApply = direction * (bounceForce + additionalForce);
            onCooldown = true;

            // If additional force is greater than 0.1, that means you're likely not at an angle, so apply a movement cooldown so the bounce feels most impactful
            if (additionalForce > 0.1f)
            {
                disableEvent.SetInputCooldown(0.05f);
                disableEvent.StopInput();
            }

            movementEvent.SetIsBounceAnimating(true);
            movementEvent.Bounce(forceToApply, ForceMode2D.Impulse);
        }
    }

    /// <summary>
    /// Check for collisions
    /// </summary>
    /// <param name="collision">The context of the collider</param>
    public override void OnCollisionEnter2D(Collision2D collision)
    {
        // Checks if the shroom hasnt already rotated
        if (!hasRotated)
        {
            // Checks if the collider is a composite collider (tilemap)
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
                if (shroomType == ShroomType.Wall)
                {
                    RotateAndFreeze(rotation);
                }
                else
                {
                    RotateAndFreeze();
                }
            }
            // Checks if the sollider is a box collider (decomposable tile or weighted platform)
            else if (collision.collider is BoxCollider2D)
            {
                // Check if the tile is decomposable
                if (collision.collider.tag == "Decomposable")
                {
                    // Set the tile to decomposed
                    if (collision.gameObject.GetComponent<DecompasableTile>().IsDecomposed == false)
                    {
                        collision.gameObject.GetComponent<DecompasableTile>().IsDecomposed = true;
                    }
                    
                    // Rotate and freeze the shroom
                    RotateAndFreeze();
                }
                // Check if the platform is weighted
                else if (collision.collider.tag == "Weighted")
                {
                    // Rotate and freeze the shroom
                    RotateAndFreeze();

                    // Check if the weight of the platform needs to be activated
                    collision.gameObject.GetComponent<MoveablePlatform>().CheckWeight(gameObject);
                }
            }
        }
    }
}

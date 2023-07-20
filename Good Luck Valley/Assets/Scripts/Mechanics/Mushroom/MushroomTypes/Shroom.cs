using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
using static UnityEngine.RuleTile.TilingRuleOutput;

public abstract class Shroom : MonoBehaviour
{
    #region REFERENCES
    protected MushroomManager mushMan;
    protected ShroomCounter shroomCounter;
    [SerializeField] GameObject shroomIcon;
    [SerializeField] protected GameObject regShroom;
    [SerializeField] protected GameObject wallShroom;
    #endregion

    #region FIELDS
    protected ShroomType shroomType;
    [SerializeField] protected MovementScriptableObj movementEvent;
    [SerializeField] protected DisableScriptableObj disableEvent;
    protected bool hasRotated;
    [SerializeField] protected float rotateAngle;
    [SerializeField] protected bool bouncing = false;
    protected float bouncingTimer = 0.1f;
    [SerializeField] protected float durationTimer;
    protected bool onScreen;
    [SerializeField] protected bool isShroom;
    protected Color defaultColor;
    protected ParticleSystem shroomParticles;
    protected bool playShroomParticle = true;
    [SerializeField] protected float particleTime;
    protected bool updateCounter;
    protected float spawnedLifeTime;
    [SerializeField] protected float bounceForce;
    [SerializeField] protected bool onCooldown = false;
    protected float cooldown = 0.1f;
    protected bool nonAnariShroom;
    protected bool flipRotation;
    #endregion

    #region PROPERTIES
    public bool HasRotated { get { return hasRotated; } set { hasRotated = value; } }
    public float RotateAngle { get { return rotateAngle; } set { rotateAngle = value; } }
    public bool Bouncing { get { return bouncing; } set { bouncing = value; } }
    public float BouncingTimer { get { return bouncingTimer; } set { bouncingTimer = value; } }
    public bool OnScreen { get { return onScreen; } set { onScreen = value; } }
    public bool IsShroom { get { return isShroom; } set { isShroom = value; } }
    public float DurationTimer { get { return durationTimer; } set { durationTimer = value; } }
    public Color ShroomColor { get { return defaultColor; } set { defaultColor = value; } }
    public ParticleSystem ShroomParticles { get { return shroomParticles; } set { shroomParticles = value; } }
    public float ParticleTime { get { return particleTime; } set { particleTime = value; } }
    public GameObject ShroomIcon { get { return shroomIcon; } set { shroomIcon = value; } }
    public float SpawnedLifeTime { get { return spawnedLifeTime; } set { spawnedLifeTime = value; } }
    public GameObject RegularMushroom { get { return regShroom; } set { regShroom = value; } }
    public GameObject WallMushroom { get { return wallShroom; } set { wallShroom = value; } }
    public bool NonAnariShroom { get { return nonAnariShroom; } set {  nonAnariShroom = value; } }
    public bool FlipRotation {  get { return flipRotation; } set {  flipRotation = value; } }
    #endregion

    public void Start()
    {
        cooldown = 0.1f;
    }

    /// <summary>
    /// Updates shroom counter's filling and timer
    /// </summary>
    public void UpdateShroomCounter()
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

        if (mushMan.MushroomLimit == 3 && !nonAnariShroom)
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

    protected void SetCooldown()
    {
        // Set a cooldown for when the player can bounce on the shroom again
        if (onCooldown)
        {
            cooldown -= Time.deltaTime;
        }

        if (cooldown <= 0)
        {
            onCooldown = false;
            cooldown = 0.1f;
        }
    }

    /// <summary>
    /// Resets the mushroom counter to filled and plays the particle effect
    /// </summary>
    /// <param name="shroomIcon"></param>
    public void ResetCounter()
    {
        ShroomIcon.GetComponent<Image>().fillAmount = 1;
        ShroomIcon.GetComponent<ParticleSystem>().Play();
    }

    /// <summary>
    /// Starts the mushroom counter's filling
    /// </summary>
    /// <param name="shroomIcon"></param>
    public void StartCounter()
    {
        ShroomIcon.GetComponent<Image>().fillAmount = 0;
    }

    /// <summary>
    /// Bounces the player in the way the shroom wants
    /// </summary>
    public abstract void Bounce();

    /// <summary>
    /// When the mushroom hits an object
    /// </summary>
    public abstract void OnCollisionEnter2D(Collision2D collision);

    /// <summary>
    /// Rotates and Freezes the mushroom 
    /// </summary>
    public void RotateAndFreeze()
    {
        // Saves the colliders of the platforms the shroom is coming into contact with intos an array
        ContactPoint2D[] contacts = new ContactPoint2D[10];
        GetComponent<CircleCollider2D>().GetContacts(contacts);

        // The direction vector that the mushroom needs to point towards,
        //      contacts[0].point is the point the shroom is touching the platform at
        //      mushroom.transform.position is the mushroom's position,
        //          casted to a vector 2 so it can be subtracted from the contact point
        ContactPoint2D contactPoint = contacts[0];
        Vector2 direction = contactPoint.normal;

        // The angle that the shroom is going to rotate at
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // The quaternion that will rotate the shroom
        Quaternion rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        transform.rotation = rotation;

        // Freezes shroom movement and rotation, and sets hasRotated to true
        GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        GetComponent<Shroom>().HasRotated = true;

        GameObject shroom;

        // Get shroom type and instantiate shrooms
        switch(shroomType)
        {
            case ShroomType.Regular:
                shroom = Instantiate(regShroom, transform.position, rotation);
                break;

            case ShroomType.Wall:
                shroom = Instantiate(wallShroom, transform.position, rotation);
                break;

            default:
                shroom = Instantiate(regShroom, transform.position, rotation); 
                break;
        }

        shroom.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        shroom.GetComponent<Shroom>().HasRotated = true;
        shroom.GetComponent<Shroom>().ShroomIcon = gameObject.GetComponent<Shroom>().ShroomIcon;
        mushMan.ChangeShroomIndexes[mushMan.MushroomList.IndexOf(gameObject)] = shroom;

        // Set the MushroomInfo angle to the calculated angle
        shroom.GetComponent<Shroom>().RotateAngle = angle;
        //if (angle >= 0)
        //{
        //    shroom.GetComponent<Shroom>().flipRotation = true;
        //}
        hasRotated = true;
    }
}

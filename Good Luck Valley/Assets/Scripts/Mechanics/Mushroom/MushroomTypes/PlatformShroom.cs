using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlatformShroom : MonoBehaviour, IShroom
{
    #region REFERENCES
    private MushroomManager mushMan;
    private ShroomCounter shroomCounter;
    [SerializeField] GameObject shroomIcon;
    [SerializeField] private GameObject mushroom;
    #endregion

    #region FIELDS
    private bool hasRotated;
    [SerializeField] float rotateAngle;
    private bool bouncing = false;
    private float bouncingTimer = 0.1f;
    [SerializeField] private float durationTimer;
    private bool onScreen;
    [SerializeField] private bool isShroom;
    private Color defaultColor;
    private ParticleSystem shroomParticles;
    private bool playShroomParticle = true;
    [SerializeField] float particleTime;
    private bool updateCounter;
    float spawnedLifeTime;
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
    public GameObject Mushroom { get { return mushroom; } set { mushroom = value; } }
    #endregion

    private void Awake()
    {
        mushMan = GameObject.Find("Mushroom Manager").GetComponent<MushroomManager>();
        shroomCounter = GameObject.Find("MushroomCountUI").GetComponent<ShroomCounter>();
        shroomParticles = GetComponent<ParticleSystem>();
        durationTimer = mushMan.ShroomDuration;
        defaultColor = new Color(168, 168, 168);
        particleTime = durationTimer;
    }

    // Update is called once per frame
    void Update()
    {
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
    }

    public void UpdateShroomCounter()
    {
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

    public void ResetCounter()
    {
        shroomIcon.GetComponent<Image>().fillAmount = 1;
        shroomIcon.GetComponent<ParticleSystem>().Play();
    }

    public void StartCounter()
    {
        shroomIcon.GetComponent<Image>().fillAmount = 0;
    }


    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasRotated)
        {
            if (collision.collider is CompositeCollider2D)
            {
                RotateAndFreeze();
            }
            else if (collision.collider is BoxCollider2D)
            {
                if (collision.collider.tag == "Decomposable")
                {
                    if (collision.gameObject.GetComponent<DecompasableTile>().IsDecomposed == false)
                    {
                        collision.gameObject.GetComponent<DecompasableTile>().IsDecomposed = true;
                    }
                    RotateAndFreeze();
                }
                else if (collision.collider.tag == "Weighted")
                {
                    RotateAndFreeze();
                    collision.gameObject.GetComponent<MoveablePlatform>().CheckWeight(gameObject);
                }
            }
        }
    }


    /// <summary>
    /// Rotates the shrooms to the normal of the plane they collide with
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
        GetComponent<MushroomInfo>().HasRotated = true;

        GameObject shroom = Instantiate(mushroom, transform.position, rotation);
        shroom.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
        shroom.GetComponent<MushroomInfo>().HasRotated = true;
        shroom.GetComponent<MushroomInfo>().ShroomIcon = gameObject.GetComponent<MushroomInfo>().ShroomIcon;
        mushMan.ChangeShroomIndexes[mushMan.MushroomList.IndexOf(gameObject)] = shroom;

        // Set the MushroomInfo angle to the calculated angle
        shroom.GetComponent<MushroomInfo>().RotateAngle = angle;
        hasRotated = true;
    }
}

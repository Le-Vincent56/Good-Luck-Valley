using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class MushroomInfo : MonoBehaviour
{
    #region REFERENCES
    private MushroomManager mushMan;
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
    private float colorPercent;
    private Image shroomTimer;
    [SerializeField] float shroomLifeTimer;
    
    // Particles
    private ParticleSystem shroomParticles;
    [SerializeField] bool isPlaying = false;


    private GameObject particles;
    #endregion

    #region PROPERTIES
    public bool HasRotated { get { return hasRotated; } set { hasRotated = value; } }
    public float RotateAngle { get { return rotateAngle; } set { rotateAngle = value; } }
    public bool Bouncing { get { return bouncing; } set { bouncing = value; } }
    public float BouncingTimer { get { return bouncingTimer; } set { bouncingTimer = value; } }
    public bool OnScreen { get { return onScreen; } set { onScreen = value; } }
    public bool IsShroom { get { return isShroom; } set { isShroom = value; } }

    public float DurationTimer { get { return durationTimer; } }
    #endregion

    private void Awake()
    {
        mushMan = GameObject.Find("Mushroom Manager").GetComponent<MushroomManager>();
        durationTimer = mushMan.ShroomDuration;
        defaultColor = GetComponent<SpriteRenderer>().color;
        colorPercent = durationTimer / 255;
        shroomLifeTimer = durationTimer;
        shroomTimer = GameObject.Find("Shroom Icon 1").GetComponentInChildren<Image>();

        // Particles
        shroomParticles = GetComponent<ParticleSystem>();
        
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

        // Decreases deltaTime from timer for this shroom
        if (mushMan.EnableShroomTimers && isShroom)
        {
            durationTimer -= Time.deltaTime;

            if (durationTimer <= (shroomLifeTimer * 0.5) && isPlaying == false)
            {
                shroomParticles.Play();
                isPlaying = true;
            }
   

            
            //GetComponent<SpriteRenderer>().color = new Color(defaultColor.r, defaultColor.g, defaultColor.b, GetComponent<SpriteRenderer>().color.a - (4/1020));
            //Debug.Log(GetComponent<SpriteRenderer>().color.a);

            //if (shroomLifeTimer > 0)
            //{
            //    shroomTimer.fillAmount = shroomLifeTimer / mushMan.ShroomDuration;
            //}

        }
        
    }


}

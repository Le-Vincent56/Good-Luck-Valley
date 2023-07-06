using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class PlatformShroom : Shroom
{
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

    public override void Bounce()
    {
        // Calculate the force to apply
        Vector3 direction;
        Quaternion rotation = Quaternion.AngleAxis(RotateAngle - 135, Vector3.forward);
        direction = rotation * Vector2.up;

        Vector3 forceToApply = direction * bounceForce;

        // Apply bounce
        mushroomEvent.SetBounce(true);
        mushroomEvent.Bounce(forceToApply, ForceMode2D.Impulse);
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        if (!hasRotated)
        {
            if (collision.collider is CompositeCollider2D)
            {
                RotateAndFreeze();
            }
        }
    }
}

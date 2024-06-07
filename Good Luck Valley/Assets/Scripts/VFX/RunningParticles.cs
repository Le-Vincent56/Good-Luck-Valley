using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class RunningParticles : MonoBehaviour
{
    // Fields
    [SerializeField] private VisualEffect grassParticle;
    [SerializeField] private VisualEffect dirtParticle;
    public float timeSinceLastParticle;
    [SerializeField] private float timeBetweenParticles;
    [SerializeField] private Transform raycastEndhardpoint;
    [SerializeField] LayerMask dirtLayer;
    [SerializeField] LayerMask grassLayer;


    private void Update()
    {
        // Need a way to check if player is grounded
        if (timeSinceLastParticle < timeBetweenParticles)
        {
            timeSinceLastParticle += Time.deltaTime;
        }
        else
        {
            CheckGroundTileAndPlayParticle();
            timeSinceLastParticle = 0;
        }
    }

    public void PlayGrassEffect()
    {
        grassParticle.Play();
    }

    public void PlayDirtEffect()
    {
        dirtParticle.Play();
    }

    private void CheckGroundTileAndPlayParticle()
    {
        RaycastHit2D dirtCast = Physics2D.Linecast(transform.position, raycastEndhardpoint.position, dirtLayer);
        RaycastHit2D grassCast = Physics2D.Linecast(transform.position, raycastEndhardpoint.position, grassLayer);

        if (grassCast)
        {
            PlayGrassEffect();
        }
        else if(dirtCast)
        {
            PlayDirtEffect();
        }
    }
}
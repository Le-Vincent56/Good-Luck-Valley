using GoodLuckValley.Player.Control;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SpawnParticleEffect : MonoBehaviour
{
    // Fields
    [Header("Particle Systems")]
    [SerializeField] private VisualEffect grassParticle;
    [SerializeField] private VisualEffect dirtParticle;

    [Header("Layer Masks")]
    [SerializeField] LayerMask dirtLayer;
    [SerializeField] LayerMask grassLayer;

    [Header("Particle Information")]
    [SerializeField] private Transform raycastEndhardpoint;


    private void Awake()
    {
        CheckGroundTileAndPlayParticle();
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
        else if (dirtCast)
        {
            PlayDirtEffect();
        }
    }
}

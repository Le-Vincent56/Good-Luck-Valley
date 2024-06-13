using GoodLuckValley.Player.Control;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.VFX;

public class RunningParticles : MonoBehaviour
{
    // Fields
    [Header("Particle Systems")]
    [SerializeField] private VisualEffect grassParticle;
    [SerializeField] private VisualEffect dirtParticle;

    [Header("Particle Information")]
    [SerializeField] private float timeSinceLastParticle;
    [SerializeField] private float timeBetweenParticles;
    [SerializeField] private float minXVelocity;
    [SerializeField] private Transform raycastEndHardpoint;

    private PlayerController playerController;
    private DetectTileType detectTileType;


    private void Awake()
    {
        playerController = GetComponentInParent<PlayerController>();
        detectTileType = new DetectTileType(this.transform.position, raycastEndHardpoint.position);
    }

    private void Update()
    {
        // Need a way to check if player is grounded
        if (timeSinceLastParticle < timeBetweenParticles)
        {
            timeSinceLastParticle += Time.deltaTime;
        }
        else if (playerController.CheckGrounded && Mathf.Abs(playerController.GetVelocity.x) > minXVelocity)
        {
            switch (detectTileType.CheckGroundTileAndPlayParticle())
            {
                case TileType.Dirt:
                    PlayEffect(dirtParticle);
                    break;
                case TileType.Grass:
                    PlayEffect(grassParticle);
                    break;
                case TileType.None:
                    Debug.Log("no tile type detected");
                    break;
            }
            timeSinceLastParticle = 0;
        }
    }

    public void PlayEffect(VisualEffect vfx)
    {
        vfx.Play();
    }
}
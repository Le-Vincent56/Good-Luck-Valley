using GoodLuckValley.Player.Control;
using GoodLuckValley.World.Tiles;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class PlayerParticlesController : MonoBehaviour
{
    [Header("Running Particles")]
    [SerializeField] private VisualEffect grassRunVFX;
    [SerializeField] private VisualEffect dirtRunVFX;
    [SerializeField] private Transform runParticlesHardpoint;

    [Header("Jump Particles")]
    [SerializeField] private VisualEffect grassJumpVFX;
    [SerializeField] private VisualEffect dirtJumpVFX;
    [SerializeField] private Transform jumpParticlesHardpoint;

    [Header("Land Particles")]
    [SerializeField] private VisualEffect grassLandVFX;
    [SerializeField] private VisualEffect dirtLandVFX;
    [SerializeField] private Transform landParticlesHardpoint;

    [Header("Bounce Airtime Particles")]
    [SerializeField] private VisualEffect bounceAirtimeVFX;
    [SerializeField] private Transform bounceAirtimeParticlesHardpoint;

    [Header("Fields")]
    [SerializeField] private float timeSinceLastParticle;
    [SerializeField] private float timeBetweenParticles;
    [SerializeField] private float minXVelocity;
    [SerializeField] private DetectTileType detectTileType;
    private PlayerController playerController;
    private bool airtimeParticlesActive;

    public PlayerController SetPlayerController { set { playerController = value; } }

    private void Awake()
    {
        detectTileType.RaycastStart = runParticlesHardpoint.position;
    }

    private void Update()
    {
        if (playerController.IsGrounded && Mathf.Abs(playerController.Velocity.x) > minXVelocity)
        {
            HandleRunningParticles();
        }

        if (airtimeParticlesActive)
        {
            bounceAirtimeVFX.SetVector2("SpawnPosition", bounceAirtimeParticlesHardpoint.position);
        }
    }

    private void HandleRunningParticles()
    {
        if (timeSinceLastParticle < timeBetweenParticles)
        {
            timeSinceLastParticle += Time.deltaTime;
        }
        else
        {
            Debug.Log("Running Particles");
            PlayEffectBasedOnTileType(grassRunVFX, dirtRunVFX, runParticlesHardpoint.position);
            timeSinceLastParticle = 0;
        }

    }

    public void HandleJumpParticles()
    {
        Debug.Log("Jumping Particles");
        PlayEffectBasedOnTileType(grassJumpVFX, dirtJumpVFX, jumpParticlesHardpoint.position);

    }
    public void HandleLandParticles()
    {
        Debug.Log("Landing Particles");
        PlayEffectBasedOnTileType(grassLandVFX, dirtLandVFX, landParticlesHardpoint.position);
    }

    public void HandleAirtimeBounceParticles()
    {
        airtimeParticlesActive = true;
        bounceAirtimeVFX.SetVector2("SpawnPosition", bounceAirtimeParticlesHardpoint.position);
        bounceAirtimeVFX.Play();
    }

    public void StopAirtimeBounceParticles()
    {
        bounceAirtimeVFX.Stop();
        airtimeParticlesActive = false;
    }

    #region HELPERS
    private void PlayEffectBasedOnTileType(VisualEffect grassEffect, VisualEffect dirtEffect, Vector2 spawnPosition)
    {
        switch (detectTileType.CheckTileType())
        {
            case TileType.Dirt:
                Debug.Log("Playing Effect: " + dirtEffect.name + " at position: " + spawnPosition);
                dirtEffect.SetVector2("SpawnPosition", spawnPosition);
                dirtEffect.Play();
                break;
            case TileType.Grass:
                Debug.Log("Playing Effect: " + grassEffect.name + " at position: " + spawnPosition);
                grassEffect.SetVector2("SpawnPosition", spawnPosition);
                grassEffect.Play();
                break;
            case TileType.None:
                Debug.Log("NONE");
                break;
        }
    }
    #endregion
}

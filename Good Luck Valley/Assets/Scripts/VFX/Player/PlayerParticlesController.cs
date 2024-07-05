using GoodLuckValley.Events;
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
    [Header("Events")]
    [SerializeField] private GameEvent onSendParticlesController;
    [SerializeField] private GameEvent onRequestHardpoints;

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

    private void Start()
    {
        // Send the particles controller
        onSendParticlesController.Raise(this, this);
    }

    private void Update()
    {
        if(!CheckHardpoints())
        {
            onRequestHardpoints.Raise(this, null);
            return;
        }

        if (playerController.IsGrounded && Mathf.Abs(playerController.Velocity.x) > minXVelocity)
        {
            HandleRunningParticles();
        }

        if (airtimeParticlesActive)
        {
            bounceAirtimeVFX.SetVector2("SpawnPosition", bounceAirtimeParticlesHardpoint.position);
        }
    }

    public void SetPlayerController(Component sender, object data)
    {
        // Verify that the correct data was sent
        if (data is not PlayerController) return;

        // Cast and set the data
        playerController = (PlayerController)data;
    }

    public bool CheckHardpoints() => (runParticlesHardpoint != null) &&
        (jumpParticlesHardpoint != null) &&
        (landParticlesHardpoint != null) &&
        (bounceAirtimeParticlesHardpoint != null);

    public void SetHardpoints(Component sender, object data)
    {
        Debug.Log("setting hardpoints");

        // Verify that the correct data was sent
        if (data is not PlayerHardpointsContainer.Hardpoints) return;

        // Cast the data
        PlayerHardpointsContainer.Hardpoints hardpoints = (PlayerHardpointsContainer.Hardpoints)data;

        // Set data
        runParticlesHardpoint = hardpoints.Run;
        jumpParticlesHardpoint = hardpoints.Jump;
        landParticlesHardpoint = hardpoints.Land;
        bounceAirtimeParticlesHardpoint = hardpoints.Bounce;
    }

    private void HandleRunningParticles()
    {
        if (timeSinceLastParticle < timeBetweenParticles)
        {
            timeSinceLastParticle += Time.deltaTime;
        }
        else
        {
            //Debug.Log("Running Particles");
            PlayEffectBasedOnTileType(grassRunVFX, dirtRunVFX, runParticlesHardpoint.position);
            timeSinceLastParticle = 0;
        }

    }

    public void HandleJumpParticles()
    {
        //Debug.Log("Jumping Particles");
        PlayEffectBasedOnTileType(grassJumpVFX, dirtJumpVFX, jumpParticlesHardpoint.position);

    }
    public void HandleLandParticles()
    {
        //Debug.Log("Landing Particles");
        PlayEffectBasedOnTileType(grassLandVFX, dirtLandVFX, landParticlesHardpoint.position);
    }

    public void HandleAirtimeBounceParticles()
    {
        if (playerController.IsGrounded)
        {
            airtimeParticlesActive = true;
            bounceAirtimeVFX.SetVector2("SpawnPosition", bounceAirtimeParticlesHardpoint.position);
            bounceAirtimeVFX.Play();    
        }
    }

    public void StopAirtimeBounceParticles()
    {
        bounceAirtimeVFX.Stop();
        airtimeParticlesActive = false; 
    }

    #region HELPERS
    private void PlayEffectBasedOnTileType(VisualEffect grassEffect, VisualEffect dirtEffect, Vector2 spawnPosition)
    {
        detectTileType.RaycastStart = spawnPosition;

        switch (detectTileType.CheckTileType())
        {
            case TileType.Dirt:
                dirtEffect.SetVector2("SpawnPosition", spawnPosition);
                dirtEffect.Play();
                break;
            case TileType.Grass:
                grassEffect.SetVector2("SpawnPosition", spawnPosition);
                grassEffect.Play();
                break;
            case TileType.None:
                Debug.Log("No Tile Type Detected");
                break;
        }
    }
    #endregion
}

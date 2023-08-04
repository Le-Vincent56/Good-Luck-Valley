using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.VFX;

public enum TileType
{
    Grass,
    Dirt,
    None
}

public class RunningEffects : MonoBehaviour
{
    #region REFERENCES
    private PlayerMovement player;
    private Rigidbody2D playerRB;
    private Tilemap tilemap;
    [SerializeField] private VisualEffect leavesParticles;
    [SerializeField] private VisualEffect dustParticles;
    [SerializeField] private VisualEffect landLeavesParticles;
    [SerializeField] private VisualEffect landDustParticles;
    [SerializeField] private VisualEffect wallSlideDustParticles;
    [SerializeField] private MovementScriptableObj movementEvent;
    [SerializeField] private GameObject groundCheckHardpoint;
    [SerializeField] private GameObject wallSlideHardpoint;
    #endregion

    #region FIELDS
    private bool createDustOnFall;
    [SerializeField] private float lowestSpeedForParticle;
    private bool jumpSoundPlayed = false;
    private bool landSoundPlayed = false;
    #endregion

    #region PROPERTIES
    #endregion

    private void Awake()
    {
        player = GetComponent<PlayerMovement>();
        playerRB = GetComponent<Rigidbody2D>();
       
        tilemap = GameObject.Find("foreground").GetComponent<Tilemap>();
    }

    private void OnEnable()
    {
        movementEvent.landEvent.AddListener(CheckLandParticle);
        movementEvent.landEvent.AddListener(PlayLandingEffect);
        movementEvent.jumpEvent.AddListener(PlayJumpingEffect);
        movementEvent.wallEvent.AddListener(PlayWallSlideEffect);
    }

    private void OnDisable()
    {
        movementEvent.landEvent.RemoveListener(CheckLandParticle);
        movementEvent.landEvent.RemoveListener(PlayLandingEffect);
        movementEvent.jumpEvent.RemoveListener(PlayJumpingEffect);
        movementEvent.wallEvent.RemoveListener(PlayWallSlideEffect);
    }

    // Update is called once per frame
    void Update()
    {
        CheckRunParticle();
        if (player.IsGrounded == false)
        {
            createDustOnFall = true;
            landSoundPlayed = false;
        } else
        {
            jumpSoundPlayed = false;
        }
    }

    /// <summary>
    /// Checks the type of tile anari is standing on
    /// </summary>
    /// <returns></returns>
    private TileType CheckTileMap()
    {
        try
        {
            if (tilemap.GetTile(tilemap.WorldToCell(groundCheckHardpoint.transform.position)).name.Contains("_d"))
            {
                return TileType.Dirt;
            }
            else if (tilemap.GetTile(tilemap.WorldToCell(groundCheckHardpoint.transform.position)).name.Contains("_g"))
            {
                return TileType.Grass;
            }
            else
            {
                return TileType.None;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to read tilemap. No particle will play. Message: " + e.Message);
            return TileType.None;
        }
    }

    /// <summary>
    /// Checks which particle to play when player lands
    /// </summary>
    private void CheckLandParticle()
    {
        if (createDustOnFall)
        {
            PlayLandingEffect();
        }
        createDustOnFall = false;
    }

    /// <summary>
    /// Check which particle to play when the player runs
    /// </summary>
    private void CheckRunParticle()
    {
        if (Mathf.Abs(playerRB.velocity.x) > lowestSpeedForParticle && player.IsGrounded)
        {
            TileType type = CheckTileMap();
            movementEvent.SetTileType(type);
            switch (type)
            {
                case TileType.Grass:
                    leavesParticles.Play();
                    break;

                case TileType.Dirt:
                    dustParticles.Play();
                    break;

                case TileType.None:
                    break;
            }
        }
    }

    /// <summary>
    /// Checks which particle to play when player lands, if we want different effects for jump and land
    /// </summary>
    private void PlayLandingEffect()
    {
        if(!landSoundPlayed)
        {
            // Check the tile map to see what TileType the player is on
            TileType type = CheckTileMap();
            movementEvent.SetTileType(type);

            // Play the corresponding sound according to the TileType
            switch (type)
            {
                case TileType.Grass:
                    landLeavesParticles.Play();
                    AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerLandGrass, transform.position);
                    landSoundPlayed = true;
                    break;

                case TileType.Dirt:
                    landDustParticles.Play();
                    AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerLandDirt, transform.position);
                    landSoundPlayed = true;
                    break;

                case TileType.None:
                    break;
            }
        }
    }

    /// <summary>
    /// Checks which particle to play when player jumps, if we want different effects for jump and land
    /// </summary>
    private void PlayJumpingEffect()
    {
        if(!jumpSoundPlayed)
        {
            // Check the tile map to see what TileType the player is on
            TileType type = CheckTileMap();
            movementEvent.SetTileType(type);

            // Play the corresponding sound according to the TileType
            switch (type)
            {
                case TileType.Grass:
                    landLeavesParticles.Play();
                    AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerLandGrass, transform.position);
                    jumpSoundPlayed = true;
                    break;

                case TileType.Dirt:
                    landDustParticles.Play();
                    AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerLandDirt, transform.position);
                    jumpSoundPlayed = true;

                    break;

                case TileType.None:
                    break;
            }
        }
    }

    /// <summary>
    /// Plays the wall sliding effect
    /// </summary>
    private void PlayWallSlideEffect()
    {
        wallSlideDustParticles.transform.position = new Vector3(movementEvent.GetWallCollisionPoint().x, wallSlideHardpoint.transform.position.y, 0);
        wallSlideDustParticles.Play();
    }
}

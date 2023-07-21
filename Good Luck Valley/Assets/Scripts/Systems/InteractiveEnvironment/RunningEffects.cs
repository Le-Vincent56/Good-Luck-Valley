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
    [SerializeField] private MovementScriptableObj movementEvent;
    [SerializeField] private GameObject groundCheckHardpoint;
    #endregion

    #region FIELDS
    private bool createDustOnFall;
    [SerializeField] private float lowestSpeedForParticle;
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
        movementEvent.jumpEvent.AddListener(PlayLandingEffect);
    }

    private void OnDisable()
    {
        movementEvent.landEvent.RemoveListener(CheckLandParticle);
        movementEvent.landEvent.RemoveListener(PlayLandingEffect);
        movementEvent.jumpEvent.RemoveListener(PlayLandingEffect);
    }

    // Update is called once per frame
    void Update()
    {
        CheckRunParticle();
        if (player.IsGrounded == false)
        {
            createDustOnFall = true;
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
    //private void CheckJumpParticle()
    //{
    //}

    private void PlayLandingEffect()
    {
        TileType type = CheckTileMap();
        movementEvent.SetTileType(type);
        switch (type)
        {
            case TileType.Grass:
                landLeavesParticles.Play();
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerLandGrass, transform.position);
                break;

            case TileType.Dirt:
                landDustParticles.Play();
                AudioManager.Instance.PlayOneShot(FMODEvents.Instance.PlayerLandDirt, transform.position);
                break;

            case TileType.None:
                break;
        }
    }
}

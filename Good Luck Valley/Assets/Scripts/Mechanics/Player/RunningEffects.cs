using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.VFX;

public class RunningEffects : MonoBehaviour
{
    enum TileType
    { 
        Grass,
        Dirt,
        None
    }

    #region REFERENCES
    private PlayerMovement player;
    private Rigidbody2D playerRB;
    private VisualEffect leavesParticles;
    private VisualEffect dustParticles;
    private VisualEffect landLeavesParticles;
    private VisualEffect landDustParticles;
    private Tilemap tilemap;
    [SerializeField] private MovementScriptableObj movementEvent;
    #endregion

    #region FIELDS
    private List<GameObject> effects;
    private GameObject groundCheckHardpoint;
    private bool createDustOnFall;
    [SerializeField] private float lowestSpeedForParticle;
    #endregion

    #region PROPERTIES
    #endregion

    private void Awake()
    {
        player = GetComponent<PlayerMovement>();
        effects = new List<GameObject>(); 
        playerRB = GetComponent<Rigidbody2D>();
        effects.AddRange(GameObject.FindGameObjectsWithTag("PlayerEffects"));
        for (int i = 0;  i < effects.Count; i++)
        {
            if (effects[i].name == "Player Dust")
            {
                dustParticles = effects[i].GetComponent<VisualEffect>();
            }
            else if (effects[i].name == "Player Leaves")
            {
                leavesParticles = effects[i].GetComponent<VisualEffect>();
            }
            else if (effects[i].name == "Player Leaves Jump")
            {
                landLeavesParticles = effects[i].GetComponent<VisualEffect>();
            }
            else if (effects[i].name == "Player Dust Jump")
            {
                landDustParticles = effects[i].GetComponent<VisualEffect>();
            }
        }
        tilemap = GameObject.Find("foreground").GetComponent<Tilemap>();
        groundCheckHardpoint = transform.GetChild(4).gameObject;
    }

    private void OnEnable()
    {
        movementEvent.landEvent.AddListener(CheckLandParticle);
        movementEvent.jumpEvent.AddListener(PlayLandingEffect);
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
                Debug.Log("hi");
                return TileType.Dirt;
            }
            else if (tilemap.GetTile(tilemap.WorldToCell(groundCheckHardpoint.transform.position)).name.Contains("_g"))
            {
                Debug.Log("what");
                return TileType.Grass;
            }
            else
            {
                Debug.Log("_+_+_+_+_");
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
        switch (type)
        {
            case TileType.Grass:
                landLeavesParticles.Play();
                break;

            case TileType.Dirt:
                landDustParticles.Play();
                break;

            case TileType.None:
                break;
        }
    }
}

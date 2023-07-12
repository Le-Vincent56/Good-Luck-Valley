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
    private Vector2 checkPos;
    [SerializeField] private float playerToGroundDistance;
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
    }

    private void OnEnable()
    {
        movementEvent.landEvent.AddListener(CheckLandParticle);
        movementEvent.jumpEvent.AddListener(CheckJumpParticle);
    }

    // Update is called once per frame
    void Update()
    {
        checkPos = transform.position - new Vector3(0, playerToGroundDistance, 0);
        CheckRunParticle();
        if (player.IsGrounded == false)
        {
            Debug.Log("Is grounded has been set to false");
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
            Debug.Log("Position: " + checkPos);
            Debug.Log(tilemap.GetTile(tilemap.WorldToCell(checkPos)));
            if (tilemap.GetTile(tilemap.WorldToCell(checkPos)).name.Contains("_d"))
            {
                return TileType.Dirt;
            }
            else if (tilemap.GetTile(tilemap.WorldToCell(checkPos)).name.Contains("_g"))
            {
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
            TileType type = CheckTileMap();
            switch (type)
            {
                case TileType.Grass:
                    Debug.Log("Fall Grass Playing");
                    landLeavesParticles.Play();
                    break;

                case TileType.Dirt:
                    Debug.Log("Fall Dirt Playing");
                    landDustParticles.Play();
                    break;

                case TileType.None:
                    Debug.Log("No particle playing");
                    break;
            }
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
                    Debug.Log("Run Grass Playing");
                    leavesParticles.Play();
                    break;

                case TileType.Dirt:
                    Debug.Log("Run Dirt Playing");
                    dustParticles.Play();
                    break;

                case TileType.None:
                    Debug.Log("No particle playing");
                    break;
            }
        }
    }

    /// <summary>
    /// Checks which particle to play when player lands
    /// </summary>
    private void CheckJumpParticle()
    {
        TileType type = CheckTileMap();
        switch (type)
        {
            case TileType.Grass:
                Debug.Log("Jump Grass Playing");
                landLeavesParticles.Play();
                break;

            case TileType.Dirt:
                Debug.Log("Jump Dirt Playing");
                landDustParticles.Play();
                break;

            case TileType.None:
                Debug.Log("No particle playing");
                break;
        }
    }
}

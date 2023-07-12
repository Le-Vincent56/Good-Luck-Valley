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
        Dirt
    }

    #region REFERENCES
    private PlayerMovement player;
    private Rigidbody2D playerRB;
    private VisualEffect grassParticles;
    private VisualEffect dirtParticles;
    private Tilemap tilemap;
    #endregion

    #region FIELDS
    private List<GameObject> effects;
    private Vector2 checkPos;
    private float distanceToSubtract;
    [SerializeField] private float playerToGroundDistance = 0.1f;
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
                dirtParticles = effects[i].GetComponent<VisualEffect>();
            }
            else if (effects[i].name == "Player Leaves")
            {
                grassParticles = effects[i].GetComponent<VisualEffect>();
            }
        }
        tilemap = GameObject.Find("foreground").GetComponent<Tilemap>();
        Debug.Log(GetComponent<BoxCollider2D>().size.y);
        distanceToSubtract = GetComponent<BoxCollider2D>().size.y + playerToGroundDistance;
        Debug.Log(distanceToSubtract);
    }

    // Update is called once per frame
    void Update()
    {
        checkPos = transform.position - new Vector3(0, distanceToSubtract, 0);
        if (Mathf.Abs(playerRB.velocity.x) > 0.001 && player.IsGrounded)
        {
            TileType type = CheckTileMap();
            switch (type)
            {
                case TileType.Grass:
                    grassParticles.Play();
                    break;

                case TileType.Dirt:
                    dirtParticles.Play();
                    break;
            }
        }    
    }

    private TileType CheckTileMap()
    {
        try
        {
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
                return TileType.Dirt;
            }
        }
        catch
        {
            Debug.LogWarning("Failed to read tilemap. Defaulting to dirt particle.");
            return TileType.Dirt;
        }
    }
}

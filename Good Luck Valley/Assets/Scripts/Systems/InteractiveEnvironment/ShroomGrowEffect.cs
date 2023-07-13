using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.VFX;

public class ShroomGrowEffect : MonoBehaviour
{
    enum TileType
    {
        Grass,
        Dirt,
        None
    }

    #region REFERENCES
    [SerializeField] private VisualEffect leavesParticles;
    [SerializeField] private VisualEffect dustParticles;
    private Tilemap tilemap;
    #endregion

    #region FIELDS
    [SerializeField] private GameObject groundCheckHardpoint;
    private bool createDustOnFall;
    #endregion

    #region PROPERTIES
    #endregion

    private void Awake()
    {
        tilemap = GameObject.Find("foreground").GetComponent<Tilemap>();
    }

    private void Start()
    {
        PlayEffect();
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

    private void PlayEffect()
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

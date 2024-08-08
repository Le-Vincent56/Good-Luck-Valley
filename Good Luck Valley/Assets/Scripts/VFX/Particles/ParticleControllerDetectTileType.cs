using GoodLuckValley.World.Tiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleControllerDetectTileType : MonoBehaviour
{
    [SerializeField] private DetectTileType detectTileType;

    public TileType GetTileType()
    {
        detectTileType.RaycastStart = transform.position;
        return detectTileType.CheckTileType();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    Grass,
    Dirt,
    None
}

public class DetectTileType
{
    private LayerMask dirtLayer = LayerMask.GetMask("Dirt");
    private LayerMask grassLayer = LayerMask.GetMask("Grass");
    private Transform raycastEnd;
    private Transform raycastStart;

    public DetectTileType(Transform raycastEnd, Transform raycastStart)
    {
        this.raycastEnd = raycastEnd;
        this.raycastStart = raycastStart;
    }

    public TileType CheckGroundTileAndPlayParticle()
    {
        RaycastHit2D dirtCast = Physics2D.Linecast(raycastStart.position, raycastEnd.position, dirtLayer);
        RaycastHit2D grassCast = Physics2D.Linecast(raycastStart.position, raycastEnd.position, grassLayer);

        if (grassCast)
        {
            return TileType.Grass;
        }
        else if (dirtCast)
        {
            return TileType.Dirt;
        }
        else
        {
            return TileType.None;
        }
    }
}

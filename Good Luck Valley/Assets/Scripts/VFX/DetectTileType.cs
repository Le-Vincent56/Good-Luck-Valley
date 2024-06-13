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
    private Vector3 raycastEnd;
    private Vector3 raycastStart;

    public DetectTileType(Vector3 raycastEnd, Vector3 raycastStart)
    {
        this.raycastEnd = raycastEnd;
        this.raycastStart = raycastStart;
    }

    public TileType CheckTileType()
    {
        RaycastHit2D dirtCast = Physics2D.Linecast(raycastStart, raycastEnd, dirtLayer);
        RaycastHit2D grassCast = Physics2D.Linecast(raycastStart, raycastEnd, grassLayer);

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

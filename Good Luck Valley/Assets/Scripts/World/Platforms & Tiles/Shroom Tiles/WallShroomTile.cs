using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallShroomTile : MonoBehaviour, IShroomeable
{
    private ShroomType shroomType = ShroomType.Wall;

    public ShroomType GetType()
    {
        return shroomType;
    }
}

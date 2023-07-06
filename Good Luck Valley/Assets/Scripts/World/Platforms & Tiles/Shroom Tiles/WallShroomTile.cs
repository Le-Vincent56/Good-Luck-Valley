using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallShroomTile : MonoBehaviour, IShroomeable
{
    #region FIELDS
    private ShroomType shroomType = ShroomType.Wall;
    #endregion

    public ShroomType GetType()
    {
        // Return the type of shroom - ShroomType.Wall
        return shroomType;
    }
}

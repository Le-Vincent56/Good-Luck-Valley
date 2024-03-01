using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoodLuckValley.Mushroom;

namespace GoodLuckValley.World.Tiles
{
    public class WallShroomTile : IShroomeable
    {
        #region FIELDS
        private readonly ShroomType shroomType = ShroomType.Wall;

        ShroomType IShroomeable.GetType()
        {
            // Return the type of shroom - ShroomType.Wall
            return shroomType;
        }
        #endregion
    }
}

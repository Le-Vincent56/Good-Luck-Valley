using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Mushroom
{
    public enum ShroomType
    {
        Regular,
        Wall
    }

    public interface IShroomeable
    {
        /// <summary>
        /// Get the type of shroom that will spawn on the tile
        /// </summary>
        /// <returns>The type of shroom that will spawn on the tile</returns>
        public ShroomType GetShroomType();
        public float GetCollisionAngle(Collision2D collision);
    }
}

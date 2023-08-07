using UnityEngine;

namespace HiveMind.Tiles
{
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
}
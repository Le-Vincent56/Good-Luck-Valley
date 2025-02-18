using System;
using UnityEngine;

namespace GoodLuckValley.Player.Movement
{
    public enum GroundType
    {
        None,
        Grass,
        Dirt,
        Stone
    }

    public enum WallType
    {
        None,
        Grass,
        Dirt,
        Stone
    }

    public class LayerDetection : MonoBehaviour
    {
        [Header("Layers")]
        private GroundType previousGroundType;
        [SerializeField] private GroundType groundType;
        private WallType previousWallType;
        [SerializeField] private WallType wallType;
        private int previousWallDirection;
        [SerializeField] private int wallDirection;
        [SerializeField] private LayerMask grassLayer;
        [SerializeField] private LayerMask dirtLayer;
        [SerializeField] private LayerMask stoneLayer;

        public Action<GroundType> OnGroundLayerChange = delegate { };
        public Action<WallType> OnWallTypeChange = delegate { };
        public Action<int> OnWallDirectionChange = delegate { };

        /// <summary>
        /// Set the Ground Layer detection
        /// </summary>
        public void SetGroundLayer(int layer)
        {
            // Set the previous ground type
            previousGroundType = groundType;

            // Set none as the current ground type
            groundType = GroundType.None;

            // Check if any of the layers are hit
            if (grassLayer == (grassLayer | (1 << layer)))
            {
                groundType = GroundType.Grass;
            }
            else if (dirtLayer == (dirtLayer | (1 << layer)))
            {
                groundType = GroundType.Dirt;
            }
            else if (stoneLayer == (stoneLayer | (1 << layer)))
            {
                groundType = GroundType.Stone;
            }

            // Exit case - the ground types are the same
            if (previousGroundType == groundType) return;

            // Notify layer change
            OnGroundLayerChange.Invoke(groundType);
        }

        /// <summary>
        /// Set the Wall Layer detection
        /// </summary>
        public void SetWallLayer(int layer)
        {
            // Set the previous wall type
            previousWallType = wallType;

            // Set none as the ground type
            wallType = WallType.None;

            // Check if any of the layers are hit
            if (grassLayer == (grassLayer | (1 << layer)))
            {
                wallType = WallType.Grass;
            }
            else if (dirtLayer == (dirtLayer | (1 << layer)))
            {
                wallType = WallType.Dirt;
            }
            else if (stoneLayer == (stoneLayer | (1 << layer)))
            {
                wallType = WallType.Stone;
            }

            // Exit case - the wall types are the same
            if (previousWallType == wallType) return;

            // Notify layer change
            OnWallTypeChange.Invoke(wallType);
        }

        /// <summary>
        /// Set the wall direction of wall layers
        /// </summary>
        public void SetWallDirection(int direction)
        {
            // Exit case - the wall direction is 0
            if (direction == 0) return;

            // Set the previous wall direction
            previousWallDirection = wallDirection;

            // Set the current wall direction
            wallDirection = direction;

            // Exit case - the wall directions are the same
            if (previousWallDirection == wallDirection) return;

            // Notify wall direction change
            OnWallDirectionChange.Invoke(wallDirection);
        }
    }
}

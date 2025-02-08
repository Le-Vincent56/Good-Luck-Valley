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
        [SerializeField] private GroundType groundType;
        [SerializeField] private WallType wallType;
        [SerializeField] private LayerMask grassLayer;
        [SerializeField] private LayerMask dirtLayer;
        [SerializeField] private LayerMask stoneLayer;

        public Action<GroundType> OnGroundLayerChange = delegate { };
        public Action<WallType> OnWallTypeChange = delegate { };

        /// <summary>
        /// Set the Ground Layer detection
        /// </summary>
        public void SetGroundLayer(int layer)
        {
            // Set none as the ground type
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

            // Notify layer change
            OnGroundLayerChange.Invoke(groundType);
        }

        /// <summary>
        /// Set the Wall Layer detection
        /// </summary>
        public void SetWallLayer(int layer)
        {
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

            // Notify layer change
            OnWallTypeChange.Invoke(wallType);
        }
    }
}

using System;
using UnityEngine;

namespace GoodLuckValley.World.Tiles
{
    public enum TileType
    {
        Grass,
        Dirt,
        Stone,
        None
    }

    public enum DetectDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    [Serializable]
    public class DetectTileType
    {
        [Header("Fields")]
        [SerializeField] private DetectDirection direction;
        [SerializeField] float distance;
        [SerializeField] LayerMask dirtLayer;
        [SerializeField] LayerMask grassLayer;
        [SerializeField] LayerMask stoneLayer;
        private Vector3 raycastStart;
        

        public Vector3 RaycastStart { get { return raycastStart; } set { raycastStart = value; } }

        public TileType CheckTileType()
        {
            Vector2 raycastDir = Vector2.down;
            switch (direction)
            {
                case DetectDirection.Up:
                    raycastDir = Vector2.up;
                    break;

                case DetectDirection.Down:
                    raycastDir = Vector2.down;
                    break;

                case DetectDirection.Left:
                    raycastDir = Vector2.left;
                    break;
                    
                case DetectDirection.Right:
                    raycastDir = Vector2.right;
                    break;
            }

            RaycastHit2D dirtCast = Physics2D.Raycast(raycastStart, raycastDir, distance, dirtLayer);
            RaycastHit2D grassCast = Physics2D.Raycast(raycastStart, raycastDir, distance, grassLayer);
            RaycastHit2D stoneCast = Physics2D.Raycast(raycastStart, raycastDir, distance, stoneLayer);

            if (grassCast)
            {
                return TileType.Grass;
            }
            else if (dirtCast)
            {
                return TileType.Dirt;
            }
            else if (stoneCast)
            {
                return TileType.Stone;
            }
            else
            {
                return TileType.None;
            }
        }
    }
}
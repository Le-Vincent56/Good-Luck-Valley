using GoodLuckValley.Mushroom;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace GoodLuckValley.World.Tiles
{
    public struct CollisionData
    {
        public enum CollisionDirection
        {
            Up = 0,
            Right = 1,
            Down = 2,
            Left = 3
        }

        public CollisionDirection Direction;
        public float Rotation;

        public CollisionData(CollisionDirection direction, float rotation)
        {
            this.Direction = direction;
            this.Rotation = rotation;
        }
    }

    public class ShroomTile : MonoBehaviour, IShroomeable
    {
        public enum TileType
        {
            Triangle = 3,
            Rectangle = 4
        }

        public enum UpDirection
        {
            Up = 0,
            Right = 1,
            Down = 2,
            Left = 3
        }

        #region FIELDS
        [SerializeField] private TileType tileType;
        [SerializeField] private ShroomType shroomType;
        [SerializeField] private UpDirection upDirection;

        [SerializeField] private Vector2 up;
        [SerializeField] private Vector2 right;
        [SerializeField] private Vector2 down;
        [SerializeField] private Vector2 left;

        [SerializeField] private float triangleTop;
        [SerializeField] private float triangleBottom;
        [SerializeField] private float triangleSide;

        [SerializeField] private float rectangleTop;
        [SerializeField] private float rectangleBottom;
        [SerializeField] private float rectangleLeft;
        [SerializeField] private float rectangleRight;
        #endregion

        private void Awake()
        {

        }

        public ShroomType GetShroomType()
        {
            return shroomType;
        }

        public CollisionData GetCollisionAngle(Collider2D collider)
        {
            Vector2 collisionDirection = (collider.attachedRigidbody.position - (Vector2)transform.position).normalized;

            float absX = Mathf.Abs(collisionDirection.x);
            float absY = Mathf.Abs(collisionDirection.y);

            CollisionData collisionData = new CollisionData(CollisionData.CollisionDirection.Up, 0f);

            // If X is greater than Y, only deal with Y
            if(absX > absY)
            {
                Vector2 directionVector = new Vector2(0, collisionDirection.y);
                if(collisionDirection.y > 0)
                {
                    collisionData.Direction = CollisionData.CollisionDirection.Up;
                    collisionData.Rotation = rectangleTop;
                } else if(collisionDirection.y < 0)
                {
                    collisionData.Direction = CollisionData.CollisionDirection.Down;
                    collisionData.Rotation = rectangleBottom;
                }
            } else if(absY > absX) // Otherwise only deal with X
            {
                Vector2 directionVector = new Vector2(collisionDirection.x, 0);
                if (collisionDirection.x > 0)
                {
                    collisionData.Direction = CollisionData.CollisionDirection.Right;
                    collisionData.Rotation = rectangleRight;
                }
                else if (collisionDirection.x < 0)
                {
                    collisionData.Direction = CollisionData.CollisionDirection.Left;
                    collisionData.Rotation = rectangleLeft;
                }
            }

            return collisionData;
        }
    }
}

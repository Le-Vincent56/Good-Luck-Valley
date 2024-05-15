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
        public Vector2 SpawnPoint;

        public CollisionData(CollisionDirection direction, float rotation, Vector2 spawnPoint)
        {
            Direction = direction;
            Rotation = rotation;
            SpawnPoint = spawnPoint;
        }
    }

    public class ShroomTile : MonoBehaviour, IShroomeable
    {
        public enum TileType
        {
            Triangle = 3,
            Rectangle = 4
        }

        #region FIELDS
        [SerializeField] private TileType tileType;
        [SerializeField] private ShroomType shroomType;

        [SerializeField] private float contactBuffer = 0.2f;

        [SerializeField] private Vector2 center;
        [SerializeField] private float width;
        [SerializeField] private float height;
        [SerializeField] private float spawnUp;
        [SerializeField] private float spawnRight;
        [SerializeField] private float spawnDown;
        [SerializeField] private float spawnLeft;

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
            if (tileType == TileType.Triangle)
            {

            }
            else if (tileType == TileType.Rectangle)
            {
                // Get bounds
                Bounds bounds = GetComponent<BoxCollider2D>().bounds;

                // Set extents and center position
                width = bounds.extents.x;
                height = bounds.extents.y;
                center = transform.position;

                // Set spawn positions
                spawnUp = center.y + height;
                spawnRight = center.x + width;
                spawnDown = center.y - height;
                spawnLeft = center.x - width;
            }
        }

        public ShroomType GetShroomType()
        {
            return shroomType;
        }

        public CollisionData GetCollisionAngle(Collider2D collider, Vector2 contactPoint)
        {
            // Create a collision data object
            CollisionData collisionData = new CollisionData(CollisionData.CollisionDirection.Up, 0f, Vector2.zero);

            // Check the direction of collision
            collisionData.Direction = CheckCollisionDirection(contactPoint);

            switch (collisionData.Direction)
            {
                // Set Up data
                case CollisionData.CollisionDirection.Up:
                    collisionData.Rotation = rectangleTop;
                    collisionData.SpawnPoint = new Vector2(contactPoint.x, spawnUp);
                    break;

                // Set Right data
                case CollisionData.CollisionDirection.Right:
                    collisionData.Rotation = rectangleRight;
                    collisionData.SpawnPoint = new Vector2(spawnRight, contactPoint.y);
                    break;

                // Set Down data
                case CollisionData.CollisionDirection.Down:
                    collisionData.Rotation = rectangleBottom;
                    collisionData.SpawnPoint = new Vector2(contactPoint.x, spawnDown);
                    break;

                // Set Left data
                case CollisionData.CollisionDirection.Left:
                    collisionData.Rotation = rectangleLeft;
                    collisionData.SpawnPoint = new Vector2(spawnLeft, contactPoint.y);
                    break;
            }

            return collisionData;
        }

        private CollisionData.CollisionDirection CheckCollisionDirection(Vector2 contactPoint)
        {
            // Check against bounds
            if (contactPoint.y < spawnUp + contactBuffer && contactPoint.y > spawnUp - contactBuffer) // Check top bound
            {
                return CollisionData.CollisionDirection.Up;
            }
            else if (contactPoint.y < spawnDown + contactBuffer && contactPoint.y > spawnDown - contactBuffer) // Check bottom bound
            {
                return CollisionData.CollisionDirection.Down;
            }
            else if (contactPoint.x < spawnRight + contactBuffer && contactPoint.x > spawnRight - contactBuffer) // Check right bound
            {
                return CollisionData.CollisionDirection.Right;
            }
            else if (contactPoint.x < spawnLeft + contactBuffer && contactPoint.x > spawnLeft - contactBuffer) // Check left bound
            {
                return CollisionData.CollisionDirection.Left;
            } else
            {
                // Default to up
                return CollisionData.CollisionDirection.Up;
            }
        }
    }
}

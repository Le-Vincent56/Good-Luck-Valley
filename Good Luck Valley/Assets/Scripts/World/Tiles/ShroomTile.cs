using GoodLuckValley.Mushroom;
using System.Collections;
using System.Collections.Generic;
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
            Left = 3,
            TopRightDiag = 4,
            TopLeftDiag = 5,
            BottomLeftDiag = 6,
            BottomRightDiag = 7,
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

        public enum PrioritySide
        {
            TopBottom = 0,
            LeftRight = 1,
        }

        public enum DiagDirection
        {
            TopRight = 0, // Rot-Z 0/360
            TopLeft = 1, // Rot-Z 90/-270
            BottomLeft = 2, // Rot-Z 180/-180
            BottomRight = 3, // Rot-Z 270/-90
        }

        #region FIELDS
        [SerializeField] private TileType tileType;
        [SerializeField] private ShroomType shroomType;
        [SerializeField] private PrioritySide prioritySide;

        [SerializeField] private Vector2 center;
        [SerializeField] private float width;
        [SerializeField] private float height;

        // Triangle fields
        [SerializeField] private float diagContactBuffer = 0.2f;
        [SerializeField] private float horContactBuffer = 0.15f;
        [SerializeField] private float verContactBuffer = 0.15f;
        [SerializeField] private DiagDirection diagDirection;
        [SerializeField] private List<Vector2> hypotenusePoints = new List<Vector2>();
        [SerializeField] private float spawnHor;
        [SerializeField] private float spawnVer;

        [SerializeField] private float diagonalRot;
        [SerializeField] private float horizontalRot;
        [SerializeField] private float verticalRot;

        private Vector2 closestPointTriangle;

        // Rectangle fields
        [SerializeField] private float rectContactBuffer = 0.2f;
        [SerializeField] private float spawnUp;
        [SerializeField] private float spawnRight;
        [SerializeField] private float spawnDown;
        [SerializeField] private float spawnLeft;

        [SerializeField] private float topRot;
        [SerializeField] private float bottomRot;
        [SerializeField] private float leftRot;
        [SerializeField] private float rightRot;
        #endregion

        private void Awake()
        {
            if (tileType == TileType.Triangle)
            {
                Bounds bounds = GetComponent<PolygonCollider2D>().bounds;

                width = bounds.extents.x;
                height = bounds.extents.y;

                // Set hypotenuse tolerance and list
                float tolerance = 0.01f;
                hypotenusePoints = new List<Vector2>();

                // Establish bounds for hypotenuse point calculation
                float xMin, xMax, yMin, yMax;

                // Set spawn positions
                switch (diagDirection)
                {
                    case DiagDirection.TopRight:
                        // Set center
                        center = new Vector2(transform.position.x - width, transform.position.y - height);

                        // Set rigid spawn points
                        spawnHor = center.y;
                        spawnVer = center.x;

                        // Set rotations
                        diagonalRot = -45f;
                        horizontalRot = 180f;
                        verticalRot = 90f;

                        // Set hypotenuse points
                        xMin = center.x;
                        xMax = center.x + (width * 2);
                        yMin = center.y;
                        yMax = center.y + (height * 2);

                        for (float i = xMin, j = yMax; 
                            i <= xMax && j >= yMin; 
                            i += tolerance, j -= tolerance)
                        {
                            hypotenusePoints.Add(new Vector2(i, j));
                        }
                        break;

                    case DiagDirection.TopLeft:
                        // Set center
                        center = new Vector2(transform.position.x + width, transform.position.y - height);

                        // Set rigid spawn points
                        spawnHor = center.y;
                        spawnVer = center.x;

                        // Set rotations
                        diagonalRot = 45f;
                        horizontalRot = 180f;
                        verticalRot = -90f;

                        // Set hypotenuse points
                        xMin = center.x - (width * 2);
                        xMax = center.x;
                        yMin = center.y;
                        yMax = center.y + (height * 2);

                        for (float i = xMin, j = yMin; 
                            i <= xMax && j <= yMax; 
                            i += tolerance, j += tolerance)
                        {
                            hypotenusePoints.Add(new Vector2(i, j));
                        }
                        break;

                    case DiagDirection.BottomRight:
                        // Set center
                        center = new Vector2(transform.position.x - width, transform.position.y + height);

                        // Set rigid spawn points
                        spawnHor = center.y;
                        spawnVer = center.x;

                        // Set rotations
                        diagonalRot = -135f;
                        horizontalRot = 0f;
                        verticalRot = 90f;

                        // Set hypotenuse points
                        xMin = center.x;
                        xMax = center.x + (width * 2);
                        yMin = center.y - (height * 2);
                        yMax = center.y;

                        for (float i = xMin, j = yMin;
                            i <= xMax && j <= yMax;
                            i += tolerance, j += tolerance)
                        {
                            hypotenusePoints.Add(new Vector2(i, j));
                        }
                        break;

                    case DiagDirection.BottomLeft:
                        // Set center
                        center = new Vector2(transform.position.x + width, transform.position.y + height);

                        // Set rigid spawn points
                        spawnHor = center.y;
                        spawnVer = center.x;

                        // Set rotations
                        diagonalRot = 135f;
                        horizontalRot = 0f;
                        verticalRot = -90f;

                        // Set hypotenuse points
                        xMin = center.x - (width * 2);
                        xMax = center.x;
                        yMax = center.y;
                        yMin = center.y - (height * 2);

                        for (float i = xMin, j = yMax;
                            i <= xMax && j >= yMin;
                            i += tolerance, j -= tolerance)
                        {
                            hypotenusePoints.Add(new Vector2(i, j));
                        }
                        break;
                }
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

                // Set rotations
                topRot = 0f;
                bottomRot = 180f;
                leftRot = 90f;
                rightRot = -90f;
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

            collisionData.Direction = CheckCollisionDirection(contactPoint);

            float closestDistance;

            switch (tileType)
            {
                case TileType.Triangle:
                    switch (collisionData.Direction)
                    {
                        case CollisionData.CollisionDirection.Up:
                            collisionData.Rotation = horizontalRot;
                            collisionData.SpawnPoint = new Vector2(contactPoint.x, spawnHor);
                            break;

                        case CollisionData.CollisionDirection.Right:
                            collisionData.Rotation = verticalRot;
                            collisionData.SpawnPoint = new Vector2(spawnVer, contactPoint.y);
                            break;

                        case CollisionData.CollisionDirection.Down:
                            collisionData.Rotation = horizontalRot;
                            collisionData.SpawnPoint = new Vector2(contactPoint.x, spawnHor);
                            break;

                        case CollisionData.CollisionDirection.Left:
                            collisionData.Rotation = verticalRot;
                            collisionData.SpawnPoint = new Vector2(spawnVer, contactPoint.y);
                            break;

                        case CollisionData.CollisionDirection.TopRightDiag:
                            // Set the rotation
                            collisionData.Rotation = diagonalRot;

                            // Find the closest hypotenuse point
                            closestDistance = 10f;
                            foreach(Vector2 point in hypotenusePoints)
                            {
                                float distance = Vector2.Distance(point, contactPoint);
                                if (distance < closestDistance)
                                {
                                    closestDistance = distance;
                                    closestPointTriangle = point;
                                }
                            }

                            // Set the spawn point to the closest hypotenuse point
                            collisionData.SpawnPoint = closestPointTriangle;
                            break;

                        case CollisionData.CollisionDirection.TopLeftDiag:
                            // Set the rotation
                            collisionData.Rotation = diagonalRot;

                            // Find the closest hypotenuse point
                            closestDistance = 10f;
                            foreach (Vector2 point in hypotenusePoints)
                            {
                                float distance = Vector2.Distance(point, contactPoint);
                                if (distance < closestDistance)
                                {
                                    closestDistance = distance;
                                    closestPointTriangle = point;
                                }
                            }

                            // Set the spawn point to the closest hypotenuse point
                            collisionData.SpawnPoint = closestPointTriangle;
                            break;

                        case CollisionData.CollisionDirection.BottomLeftDiag:
                            // Set the rotation
                            collisionData.Rotation = diagonalRot;

                            // Find the closest hypotenuse point
                            closestDistance = 10f;
                            foreach (Vector2 point in hypotenusePoints)
                            {
                                float distance = Vector2.Distance(point, contactPoint);
                                if (distance < closestDistance)
                                {
                                    closestDistance = distance;
                                    closestPointTriangle = point;
                                }
                            }

                            // Set the spawn point to the closest hypotenuse point
                            collisionData.SpawnPoint = closestPointTriangle;
                            break;

                        case CollisionData.CollisionDirection.BottomRightDiag:
                            // Set the rotation
                            collisionData.Rotation = diagonalRot;

                            // Find the closest hypotenuse point
                            closestDistance = 10f;
                            foreach (Vector2 point in hypotenusePoints)
                            {
                                float distance = Vector2.Distance(point, contactPoint);
                                if (distance < closestDistance)
                                {
                                    closestDistance = distance;
                                    closestPointTriangle = point;
                                }
                            }

                            // Set the spawn point to the closest hypotenuse point
                            collisionData.SpawnPoint = closestPointTriangle;
                            break;
                    }
                    break;

                case TileType.Rectangle:
                    switch (collisionData.Direction)
                    {
                        // Set Up data
                        case CollisionData.CollisionDirection.Up:
                            collisionData.Rotation = topRot;
                            collisionData.SpawnPoint = new Vector2(contactPoint.x, spawnUp);
                            break;

                        // Set Right data
                        case CollisionData.CollisionDirection.Right:
                            collisionData.Rotation = rightRot;
                            collisionData.SpawnPoint = new Vector2(spawnRight, contactPoint.y);
                            break;

                        // Set Down data
                        case CollisionData.CollisionDirection.Down:
                            collisionData.Rotation = bottomRot;
                            collisionData.SpawnPoint = new Vector2(contactPoint.x, spawnDown);
                            break;

                        // Set Left data
                        case CollisionData.CollisionDirection.Left:
                            collisionData.Rotation = leftRot;
                            collisionData.SpawnPoint = new Vector2(spawnLeft, contactPoint.y);
                            break;
                    }
                    break;
            }

            return collisionData;
        }

        private CollisionData.CollisionDirection CheckCollisionDirection(Vector2 contactPoint)
        {
            switch (tileType)
            {
                case TileType.Triangle:

                    switch (diagDirection)
                    {
                        case DiagDirection.TopRight:
                            return CheckTriangleCollision(contactPoint, CollisionData.CollisionDirection.TopRightDiag, CollisionData.CollisionDirection.Down, CollisionData.CollisionDirection.Left);

                        case DiagDirection.TopLeft:
                            return CheckTriangleCollision(contactPoint, CollisionData.CollisionDirection.TopLeftDiag, CollisionData.CollisionDirection.Down, CollisionData.CollisionDirection.Right);

                        case DiagDirection.BottomLeft:
                            return CheckTriangleCollision(contactPoint, CollisionData.CollisionDirection.BottomLeftDiag, CollisionData.CollisionDirection.Up, CollisionData.CollisionDirection.Right);

                        case DiagDirection.BottomRight:
                            return CheckTriangleCollision(contactPoint, CollisionData.CollisionDirection.BottomRightDiag, CollisionData.CollisionDirection.Up, CollisionData.CollisionDirection.Left);
                    }
                    break;

                case TileType.Rectangle:
                    // Check tile priority
                    if(prioritySide == PrioritySide.TopBottom)
                    {
                        // Check against bounds
                        if (contactPoint.y < spawnUp + rectContactBuffer && contactPoint.y > spawnUp - rectContactBuffer) // Check top bound
                        {
                            return CollisionData.CollisionDirection.Up;
                        }
                        else if (contactPoint.y < spawnDown + rectContactBuffer && contactPoint.y > spawnDown - rectContactBuffer) // Check bottom bound
                        {
                            return CollisionData.CollisionDirection.Down;
                        }
                        else if (contactPoint.x < spawnRight + rectContactBuffer && contactPoint.x > spawnRight - rectContactBuffer) // Check right bound
                        {
                            return CollisionData.CollisionDirection.Right;
                        }
                        else if (contactPoint.x < spawnLeft + rectContactBuffer && contactPoint.x > spawnLeft - rectContactBuffer) // Check left bound
                        {
                            return CollisionData.CollisionDirection.Left;
                        }
                    } else if(prioritySide == PrioritySide.LeftRight)
                    {
                        if (contactPoint.x < spawnRight + rectContactBuffer && contactPoint.x > spawnRight - rectContactBuffer) // Check right bound
                        {
                            return CollisionData.CollisionDirection.Right;
                        }
                        else if (contactPoint.x < spawnLeft + rectContactBuffer && contactPoint.x > spawnLeft - rectContactBuffer) // Check left bound
                        {
                            return CollisionData.CollisionDirection.Left;
                        }
                        else if (contactPoint.y < spawnUp + rectContactBuffer && contactPoint.y > spawnUp - rectContactBuffer) // Check top bound
                        {
                            return CollisionData.CollisionDirection.Up;
                        }
                        else if (contactPoint.y < spawnDown + rectContactBuffer && contactPoint.y > spawnDown - rectContactBuffer) // Check bottom bound
                        {
                            return CollisionData.CollisionDirection.Down;
                        }
                    }
                    
                    break;
            }

            // Default to up
            return CollisionData.CollisionDirection.Up;
        }

        private CollisionData.CollisionDirection CheckTriangleCollision(Vector2 contactPoint, 
            CollisionData.CollisionDirection diagDirection, CollisionData.CollisionDirection horizontal, CollisionData.CollisionDirection vertical)
        {
            // Iterate through the hypotenuse points, prioritizing the diagonal
            foreach (Vector2 point in hypotenusePoints)
            {
                if ((contactPoint.x < point.x + diagContactBuffer && contactPoint.x > point.x - diagContactBuffer)
                    && (contactPoint.y < point.y + diagContactBuffer && contactPoint.y > point.y - diagContactBuffer))
                {
                    // Return the diagonal
                    return diagDirection;
                }
            }

            // Check which side to prioritize
            if (prioritySide == PrioritySide.TopBottom)
            {
                // Prioritize the horizontals over the verticals
                if (contactPoint.y < spawnHor + horContactBuffer && contactPoint.y > spawnHor - horContactBuffer)
                {
                    return horizontal;
                }
                else if (contactPoint.x < spawnVer + verContactBuffer && contactPoint.x > spawnVer - verContactBuffer)
                {
                    return vertical;
                }
            } else if(prioritySide == PrioritySide.LeftRight)
            {
                // Prioritize the verticals over the horizontals
                if (contactPoint.x < spawnVer + verContactBuffer && contactPoint.x > spawnVer - verContactBuffer)
                {
                    return vertical;
                }
                else if (contactPoint.y < spawnHor + horContactBuffer && contactPoint.y > spawnHor - horContactBuffer)
                {
                    return horizontal;
                }
            }

            // If no cases, pass horizontal
            return horizontal;
        }

        private void OnDrawGizmos()
        {
            if(tileType == TileType.Triangle)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(center, 0.1f);

                if(hypotenusePoints.Count > 0)
                {
                    foreach(Vector2 point in hypotenusePoints)
                    {
                        Gizmos.DrawSphere(point, 0.01f);
                    }
                }
            }
        }
    }
}

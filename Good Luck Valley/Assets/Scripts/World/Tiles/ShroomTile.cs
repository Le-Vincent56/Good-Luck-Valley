using GoodLuckValley.Mushroom;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.World.Tiles
{
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


        [SerializeField] private float triangleTop;
        [SerializeField] private float triangleBottom;
        [SerializeField] private float triangleSide;

        [SerializeField] private float rectangleTop;
        [SerializeField] private float rectangleBottom;
        [SerializeField] private float rectangleLeft;
        [SerializeField] private float rectangleRight;
        #endregion

        public ShroomType GetShroomType()
        {
            return shroomType;
        }

        public float GetCollisionAngle(Collision2D collision)
        {
            ContactPoint2D contact = collision.contacts[0];
            Vector2 normal = contact.normal;

            // Calculate the angle between the collision normal and a reference direction
            float angle = Vector2.SignedAngle(Vector2.up, normal);

            switch(tileType)
            {
                case TileType.Triangle:
                    // Collision happened on the top
                    if(Mathf.Approximately(angle, 0.0f))
                    {
                        return triangleTop;
                        // Collision happened on the side
                    } else if(Mathf.Approximately(angle, 90.0f) ||  Mathf.Approximately(angle, -90.0f))
                    {
                        return triangleSide;
                    } else
                    {
                        // Collision happened on the bottom
                        return triangleBottom;
                    }

                case TileType.Rectangle:
                    // Return angle based on collision side
                    if (Mathf.Approximately(Mathf.Abs(angle), 90.0f))
                    {
                        // Collision happened on top or bottom
                        if (normal.y > 0)
                            return rectangleTop;
                        else
                            return rectangleBottom;
                    }
                    // Collision happened on the left or right
                    else if (Mathf.Approximately(angle, 0.0f) || Mathf.Approximately(angle, 180.0f))
                    {
                        if (normal.x > 0)
                            return rectangleRight;
                        else
                            return rectangleLeft;
                    }
                    else
                    {
                        // Return a default value
                        return 0f;
                    }

                default:
                    return 0f;
            }
        }
    }
}

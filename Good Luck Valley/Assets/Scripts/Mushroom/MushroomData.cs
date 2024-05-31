using GoodLuckValley.World.Tiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Mushroom
{
    public class MushroomData : MonoBehaviour
    {
        #region FIELDS
        [SerializeField] private ShroomType shroomType;
        [SerializeField] private Vector2 bounceDirection;
        [SerializeField] private float bounceForce;
        [SerializeField] private float rotation;
        #endregion

        #region PROPERTIES
        public bool Rotated { get; private set; }
        #endregion

        /// <summary>
        /// Instantiate the data of the Mushroom
        /// </summary>
        /// <param name="shroomType"></param>
        public void InstantiateMushroomData(ShroomType shroomType, float rotation)
        {
            // Set data
            this.shroomType = shroomType;
            this.rotation = rotation;

            // Decide bounce force based on shroom type
            switch (shroomType)
            {
                case ShroomType.Regular:
                    // Set bounce direction and force
                    bounceDirection = transform.up.normalized;
                    bounceForce = 10f;
                    Rotated = false;

                    if(Mathf.Abs((int)rotation) == 45)
                    {
                        bounceForce = 10f / Mathf.Sin(45f);
                        Rotated = true;
                    }
                    break;

                case ShroomType.Wall:
                    Quaternion bounceRotation;

                    // Check rotations
                    if(rotation > 0)
                    {
                        bounceRotation = Quaternion.AngleAxis(rotation - 135, Vector3.forward);
                    } else
                    {
                        bounceRotation = Quaternion.AngleAxis(rotation + 135, Vector3.forward);
                    }

                    // Set bounce direction and force
                    bounceDirection = (bounceRotation * transform.up).normalized;
                    bounceForce = 15f;
                    break;
            }
        }

        /// <summary>
        /// Get the Bounce Vector of the Mushroom
        /// </summary>
        /// <returns></returns>
        public Vector2 GetBounceVector()
        {
            return bounceDirection * bounceForce;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, bounceDirection);
        }
    }
}

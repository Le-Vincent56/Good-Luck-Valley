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
        #endregion

        /// <summary>
        /// Instantiate the data of the Mushroom
        /// </summary>
        /// <param name="shroomType"></param>
        public void InstantiateMushroomData(ShroomType shroomType)
        {
            // Set data
            this.shroomType = shroomType;
            bounceDirection = transform.up.normalized;

            // Decide bounce force based on shroom type
            switch (shroomType)
            {
                case ShroomType.Regular:
                    bounceForce = 10f;
                    break;

                case ShroomType.Wall:
                    bounceForce = 10f;
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

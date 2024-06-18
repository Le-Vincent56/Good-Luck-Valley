using UnityEngine;

namespace GoodLuckValley.Mushroom
{
    public class MushroomInfo : MonoBehaviour
    {
        [Header("Events")]
        [SerializeField] private MushroomData mushroomData;

        [Header("Fields")]
        [SerializeField] private ShroomType shroomType;
        [SerializeField] private Vector2 bounceDirection;
        [SerializeField] private float bounceForce;
        [SerializeField] private int rotation;

        #region PROPERTIES
        public bool Rotated { get; private set; }
        #endregion

        /// <summary>
        /// Instantiate the data of the Mushroom
        /// </summary>
        /// <param name="shroomType"></param>
        public void InstantiateMushroomData(ShroomType shroomType, int rotation)
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
                    bounceForce = mushroomData.regularBounceForce;
                    Rotated = false;

                    if(Mathf.Abs(rotation) == 45 || Mathf.Abs(rotation) == 135)
                    {
                        bounceForce = mushroomData.regularSlopeBounceForce;
                        bounceDirection.x -= 0.2f;
                        bounceDirection.y += 0.2f;
                        Debug.Log("Mushroom Direction: " + bounceDirection);
                        Rotated = true;
                    }
                    break;

                case ShroomType.Wall:
                    Quaternion bounceRotation;

                    // Check rotations
                    if(rotation > 0)
                    {
                        bounceRotation = Quaternion.AngleAxis(rotation - 150, Vector3.forward);
                    } else
                    {
                        bounceRotation = Quaternion.AngleAxis(rotation + 150, Vector3.forward);
                    }

                    // Set bounce direction and force
                    bounceDirection = (bounceRotation * transform.up).normalized;
                    bounceForce = mushroomData.wallBounceForce;
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

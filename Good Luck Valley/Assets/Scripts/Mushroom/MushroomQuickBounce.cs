using GoodLuckValley.Events;
using GoodLuckValley.World.Tiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Mushroom
{
    public class MushroomQuickBounce : MonoBehaviour
    {
        #region EVENTS
        [Header("Events")]
        [SerializeField] private GameEvent onAddMushroom;
        #endregion

        #region REFERENCES
        [SerializeField] private GameObject regShroom;
        #endregion

        #region FIELDS
        [Header("Fields")]
        [SerializeField] private float maxDistance;
        [SerializeField] private LayerMask shroomableLayer;
        #endregion

        /// <summary>
        /// Handle the Quick Bounce
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void QuickBounce(Component sender, object data)
        {
            // Raycast down and look for shroomable surfaces
            RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.down, maxDistance, shroomableLayer);

            // If hit
            if(hitInfo)
            {
                // Attempt to get a shroom tile
                ShroomTile shroomTile = hitInfo.transform.gameObject.GetComponent<ShroomTile>();

                // If there's a tile, create a mushroom
                if (shroomTile != null)
                {
                    // Gather collision data
                    CollisionData collisionData = shroomTile.GetCollisionAngle(hitInfo.point);

                    // Create the mushroom
                    CreateShroom(collisionData);
                }
            }
        }

        /// <summary>
        /// Create a Mushroom
        /// </summary>
        /// <param name="collisionData">The CollisionData in which to create the Mushroom with</param>
        public void CreateShroom(CollisionData collisionData)
        {
            float shroomHeight = (regShroom.GetComponent<SpriteRenderer>().bounds.size.y / 2) - 0.035f;
            float shroomHeightDiag = shroomHeight * (3f / 4f);

            // The quaternion that will rotate the shroom
            Quaternion rotationQuat = Quaternion.AngleAxis(collisionData.Rotation, Vector3.forward);

            // Displace the shroom depending on collision direction
            switch (collisionData.Direction)
            {
                case CollisionData.CollisionDirection.Up:
                    collisionData.SpawnPoint.y += shroomHeight;
                    break;

                case CollisionData.CollisionDirection.Right:
                    collisionData.SpawnPoint.x += shroomHeight;
                    break;

                case CollisionData.CollisionDirection.Down:
                    collisionData.SpawnPoint.y -= shroomHeight;
                    break;

                case CollisionData.CollisionDirection.Left:
                    collisionData.SpawnPoint.x -= shroomHeight;
                    break;

                case CollisionData.CollisionDirection.TopRightDiag:
                    collisionData.SpawnPoint.x += shroomHeightDiag;
                    collisionData.SpawnPoint.y += shroomHeightDiag;
                    break;

                case CollisionData.CollisionDirection.TopLeftDiag:
                    collisionData.SpawnPoint.x -= shroomHeightDiag;
                    collisionData.SpawnPoint.y += shroomHeightDiag;
                    break;

                case CollisionData.CollisionDirection.BottomLeftDiag:
                    collisionData.SpawnPoint.x -= shroomHeightDiag;
                    collisionData.SpawnPoint.y -= shroomHeightDiag;
                    break;

                case CollisionData.CollisionDirection.BottomRightDiag:
                    collisionData.SpawnPoint.x += shroomHeightDiag;
                    collisionData.SpawnPoint.y -= shroomHeightDiag;
                    break;
            }

            // Instantiate the regular shroom
            GameObject shroom = Instantiate(regShroom, collisionData.SpawnPoint, rotationQuat);
            shroom.GetComponent<MushroomData>().InstantiateMushroomData(ShroomType.Regular, collisionData.Rotation);

            // Add the Mushroom to its respective list
            // Calls to:
            //  - MushroomTracker.AddMushroom();
            onAddMushroom.Raise(this, shroom);
        }
    }
}
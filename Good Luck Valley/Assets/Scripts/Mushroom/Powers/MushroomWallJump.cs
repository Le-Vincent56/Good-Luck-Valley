using GoodLuckValley.Events;
using GoodLuckValley.World.Tiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Mushroom
{
    public class MushroomWallJump : MonoBehaviour
    {
        public struct Data
        {
            public bool Input;
            public Vector2 WallCheckPos;
            public float Direction;
            public float Size;

            public Data(bool input, Vector2 wallCheckPos, float direction, float size)
            {
                Input = input;
                WallCheckPos = wallCheckPos;
                Direction = direction;
                Size = size;
            }
        }

        #region EVENTS
        [Header("Events")]
        [SerializeField] private GameEvent onRequestUnlockedWallJump;
        [SerializeField] private GameEvent onWallJump;
        [SerializeField] private GameEvent onAddWallMushroom;
        [Space(5f)]
        #endregion

        #region REFERENCES
        [Header("References")]
        [SerializeField] private GameObject wallShroom;
        [Space(5f)]
        #endregion

        #region FIELDS
        [Header("Fields")]
        [SerializeField] private bool unlockedWallJump;
        [SerializeField] private LayerMask mushroomWallLayer;
        private bool wallJumpInput;
        private float wallDirection;
        private float size;
        private Vector2 contactPoint;
        #endregion

        /// <summary>
        /// Create a Wall Jumnp Mushroom
        /// </summary>
        /// <param name="collisionData">The CollisionData to inform Mushroom placement</param>
        public GameObject CreateShroom(CollisionData collisionData)
        {
            // Get the shroom height
            float shroomHeight = (wallShroom.GetComponent<SpriteRenderer>().bounds.size.y / 2) - 0.25f;

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
            }

            // Create a shroom
            GameObject shroom = Instantiate(wallShroom, collisionData.SpawnPoint, rotationQuat);
            shroom.GetComponent<MushroomInfo>().InstantiateMushroomData(ShroomType.Wall, collisionData.Rotation);

            // Add the wall mushroom to its respective list
            // Calls to:
            //  - MushroomTracker.AddWallMushroom();
            onAddWallMushroom.Raise(this, shroom);
            return shroom;
        }

        /// <summary>
        /// Get the wall direction of the wall jump and prepare the jump
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void GetSpawnData(Component sender, object data)
        {
            // See if the wall jump is unlocked
            onRequestUnlockedWallJump.Raise(this, null);

            // Check the correct data was sent or if the wall jump is unlocked
            if (data is not RaycastHit2D || !unlockedWallJump) return;

            RaycastHit2D hit = (RaycastHit2D)data;

            // Set contact point
            contactPoint = hit.point;

            // Create collision data
            CollisionData collisionData = hit.transform.gameObject.GetComponent<ShroomTile>().GetCollisionAngle(contactPoint);

            // Create the mushroom
            GameObject shroom = CreateShroom(collisionData);

            // Set bounce data
            Vector2 wallJumpVector = shroom.GetComponent<MushroomInfo>().GetBounceVector();

            // Apply force
            // Calls to:
            //  - PlayerController.StartWallJump()
            onWallJump.Raise(this, wallJumpVector);
        }

        /// <summary>
        /// Set if the wall jump ability is unlocked
        /// </summary>
        /// <param name="unlockedWallJump">Whether or not the wall jump is unlocked</param>
        public void SetWallJumpUnlocked(bool unlockedWallJump)
        {
            this.unlockedWallJump = unlockedWallJump;
        }

        private void OnDrawGizmos()
        {
            if(wallJumpInput)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawLine(transform.position, new Vector2(transform.position.x + (wallDirection * size), transform.position.y));
            }

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(contactPoint, 0.15f);
        }
    }
}

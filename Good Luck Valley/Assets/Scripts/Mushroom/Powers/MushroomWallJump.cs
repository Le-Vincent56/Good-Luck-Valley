using GoodLuckValley.Events;
using UnityEditor.PackageManager;
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

            if(hit)
            {
                // Create a spawn data container
                ShroomSpawnData spawnData = new ShroomSpawnData();

                // Set collision data
                spawnData.Point = hit.point;
                spawnData.Rotation = (int)Vector2.Angle(hit.normal, Vector2.up) * -(int)Mathf.Sign(hit.normal.x);
                spawnData.Valid = true;

                //  Calculate and store the final spawn info
                FinalSpawnInfo finalSpawnInfo = new FinalSpawnInfo();
                CalculateFinalSpawnInfo(spawnData, ref finalSpawnInfo);

                // Create the mushroom
                CreateShroom(finalSpawnInfo);

                // Create the mushroom
                GameObject shroom = CreateShroom(finalSpawnInfo);

                // Set bounce data
                Vector2 wallJumpVector = shroom.GetComponent<MushroomInfo>().GetBounceVector();

                // Apply force
                // Calls to:
                //  - PlayerController.StartWallJump()
                onWallJump.Raise(this, wallJumpVector);
            }
        }

        /// <summary>
        /// Calculate the final spawn info for the mushroom wall jump
        /// </summary>
        /// <param name="spawnData">The spawn data of the mushroom</param>
        /// <param name="finalSpawnInfo">The final spawn info to store into</param>
        public void CalculateFinalSpawnInfo(ShroomSpawnData spawnData, ref FinalSpawnInfo finalSpawnInfo)
        {
            // Rotation of the shroom according to collision data
            Quaternion rotationQuat = Quaternion.AngleAxis(spawnData.Rotation, Vector3.forward);
            transform.rotation = rotationQuat;

            // Set spawn info
            finalSpawnInfo = new FinalSpawnInfo(spawnData.Point, rotationQuat, spawnData.Rotation);
        }

        /// <summary>
        /// Create a Wall Jumnp Mushroom
        /// </summary>
        /// <param name="finalSpawnInfo">The final spawn info of the mushroom</param>
        public GameObject CreateShroom(FinalSpawnInfo finalSpawnInfo)
        {
            // Create a shroom
            GameObject shroom = Instantiate(wallShroom, finalSpawnInfo.Position, finalSpawnInfo.Rotation);

            // Get the mushroom's height
            Bounds bounds = shroom.GetComponent<BoxCollider2D>().bounds;
            float mushroomHeight = (bounds.max.y - bounds.min.y) / 6f;

            // Get the spawn point
            Vector2 spawnPoint = finalSpawnInfo.Position;

            Debug.Log(finalSpawnInfo.Angle);

            // Edit spawn point position depending on angle
            switch (finalSpawnInfo.Angle)
            {
                case 90:
                    spawnPoint.x -= mushroomHeight;
                    break;

                case -90:
                    spawnPoint.x += mushroomHeight;
                    break;
            }

            // Set the spawn point
            shroom.transform.position = spawnPoint;

            // Set mushroom info
            shroom.GetComponent<MushroomInfo>().InstantiateMushroomData(ShroomType.Wall, finalSpawnInfo.Angle);

            // Add the wall mushroom to its respective list
            // Calls to:
            //  - MushroomTracker.AddWallMushroom();
            onAddWallMushroom.Raise(this, shroom);
            return shroom;
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

using GoodLuckValley.Events;
using UnityEngine;

namespace GoodLuckValley.Mushroom
{
    public class MushroomQuickBounce : MonoBehaviour
    {
        #region EVENTS
        [Header("Events")]
        [SerializeField] private GameEvent onAddMushroom;
        [SerializeField] private GameEvent onRequestThrowUnlock;
        #endregion

        #region REFERENCES
        [SerializeField] private GameObject quickShroom;
        #endregion

        #region FIELDS
        [Header("Fields")]
        [SerializeField] private float maxDistance;
        [SerializeField] private LayerMask shroomableLayer;
        private bool throwUnlocked;
        #endregion

        /// <summary>
        /// Handle the Quick Bounce
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void QuickBounce(Component sender, object data)
        {
            // Check to see if the throw is unlocked
            onRequestThrowUnlock.Raise(this, null);

            // If the throw is not unlocked, return
            if (!throwUnlocked) return;

            // Raycast down and look for shroomable surfaces
            RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, Vector2.down, maxDistance, shroomableLayer);

            // If hit
            if(hitInfo)
            {
                // Create a spawn data container
                ShroomSpawnData spawnData = new ShroomSpawnData();

                // Set collision data
                spawnData.Point = hitInfo.point;
                spawnData.Rotation = (int)Vector2.Angle(hitInfo.normal, Vector2.up) * -(int)Mathf.Sign(hitInfo.normal.x);
                spawnData.Valid = true;

                //  Calculate and store the final spawn info
                FinalSpawnInfo finalSpawnInfo = new FinalSpawnInfo();
                CalculateFinalSpawnInfo(spawnData, ref finalSpawnInfo);

                // Create the mushroom
                CreateShroom(finalSpawnInfo);
            }
        }

        /// <summary>
        ///  Calculate the final spawn info for the mushroom
        /// </summary>
        /// <param name="spawnData"></param>
        /// <param name="finalSpawnInfo"></param>
        public void CalculateFinalSpawnInfo(ShroomSpawnData spawnData, ref FinalSpawnInfo finalSpawnInfo)
        {
            // Rotation of the shroom according to collision data
            Quaternion rotationQuat = Quaternion.AngleAxis(spawnData.Rotation, Vector3.forward);
            transform.rotation = rotationQuat;

            // Set spawn info
            finalSpawnInfo = new FinalSpawnInfo(spawnData.Point, rotationQuat, spawnData.Rotation);
        }

        /// <summary>
        /// Create a Mushroom
        /// </summary>
        /// <param name="collisionData">The CollisionData in which to create the Mushroom with</param>
        public void CreateShroom(FinalSpawnInfo finalSpawnInfo)
        {
            // Instantiate the regular shroom
            GameObject shroom = Instantiate(quickShroom, finalSpawnInfo.Position, finalSpawnInfo.Rotation);

            // Get the height of the mushroom
            Bounds bounds = shroom.GetComponent<BoxCollider2D>().bounds;
            float mushroomHeight = (bounds.max.y - bounds.min.y) / 2f;

            // Get the spawn point
            Vector2 spawnPoint = finalSpawnInfo.Position;

            // Edit spawn point position depending on angle
            switch (finalSpawnInfo.Angle)
            {
                case 0:
                    spawnPoint.y += mushroomHeight;
                    break;

                case 90:
                    spawnPoint.x -= mushroomHeight;
                    break;

                case -90:
                    spawnPoint.x += mushroomHeight;
                    break;

                case 44:
                    spawnPoint.x -= mushroomHeight * Mathf.Cos(45) * 0.7f;
                    spawnPoint.y += mushroomHeight * Mathf.Sin(45) * 0.7f;
                    break;

                case 45:
                    spawnPoint.x -= mushroomHeight * Mathf.Cos(45) * 0.7f;
                    spawnPoint.y += mushroomHeight * Mathf.Sin(45) * 0.7f;
                    break;

                case -44:
                    spawnPoint.x += mushroomHeight * Mathf.Cos(45);
                    spawnPoint.y += mushroomHeight * Mathf.Sin(45);
                    break;

                case -45:
                    spawnPoint.x += mushroomHeight * Mathf.Cos(45);
                    spawnPoint.y += mushroomHeight * Mathf.Sin(45);
                    break;
            }

            // Set the shroom position
            shroom.transform.position = spawnPoint;

            // Set mushroom info
            shroom.GetComponent<MushroomInfo>().InstantiateMushroomData(ShroomType.Quick, finalSpawnInfo.Angle);

            // Add the Mushroom to its respective list
            // Calls to:
            //  - MushroomTracker.AddMushroom();
            onAddMushroom.Raise(this, shroom);
        }

        /// <summary>
        /// Set if the Mushroom Throw is unlocked
        /// </summary>
        /// <param name="throwUnlocked">Whether or not the Mushroom Throw is unlocked</param>
        public void SetThrowUnlocked(bool throwUnlocked)
        {
            this.throwUnlocked = throwUnlocked;
        }
    }
}
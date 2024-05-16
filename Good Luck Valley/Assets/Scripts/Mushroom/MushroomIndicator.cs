using GoodLuckValley.World.Tiles;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Mushroom
{
    public class MushroomIndicator : MonoBehaviour
    {

        #region EVENTS
        #endregion

        #region REFERENCES
        private SpriteRenderer spriteRenderer;
        #endregion

        #region FIELDS
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            // Get the sprite renderer
            spriteRenderer = GetComponent<SpriteRenderer>();

            // Set the sprite to be invisible
            SetSpriteOpacity(0f);
        }

        /// <summary>
        /// Places a mushroom indicator at the end of the aim line using the given 
        ///     collision data from the collided tiled
        /// </summary>
        /// <param name="sender"></param>
        /// /// <param name="data"></param>
        public void ShowIndicator(Component sender, object data)
        {
            // Return if not the correct data type
            if (data is not CollisionData) return;

            // Cast the data
            CollisionData collisionData = (CollisionData)data;

            // Show Indicator
            SetSpriteOpacity(1f);

            // Height of the shroom for offsetting
            float shroomHeight = 0.226875f;
            float shroomHeightDiag = shroomHeight * (3f / 4f);

            // Rotation of the shroom according to collision data
            Quaternion rotationQuat = Quaternion.AngleAxis(collisionData.Rotation, Vector3.forward);
            transform.rotation = rotationQuat;

            // Moves shroom indicator to end of line position
            Vector2 spawnPoint = collisionData.SpawnPoint;

            // Displace the shroom depending on collision direction
            switch (collisionData.Direction)
            {
                case CollisionData.CollisionDirection.Up:
                    spawnPoint.y += shroomHeight;
                    break;

                case CollisionData.CollisionDirection.Right:
                    spawnPoint.x += shroomHeight;
                    break;

                case CollisionData.CollisionDirection.Down:
                    spawnPoint.y -= shroomHeight;
                    break;

                case CollisionData.CollisionDirection.Left:
                    spawnPoint.x -= shroomHeight;
                    break;

                case CollisionData.CollisionDirection.TopRightDiag:
                    spawnPoint.x += shroomHeightDiag;
                    spawnPoint.y += shroomHeightDiag;
                    break;

                case CollisionData.CollisionDirection.TopLeftDiag:
                    spawnPoint.x -= shroomHeightDiag;
                    spawnPoint.y += shroomHeightDiag;
                    break;

                case CollisionData.CollisionDirection.BottomLeftDiag:
                    spawnPoint.x -= shroomHeightDiag;
                    spawnPoint.y -= shroomHeightDiag;
                    break;

                case CollisionData.CollisionDirection.BottomRightDiag:
                    spawnPoint.x += shroomHeightDiag;
                    spawnPoint.y -= shroomHeightDiag;
                    break;
            }

            // Set the shroom inidcator position
            transform.position = spawnPoint;
        }

        /// <summary>
        /// Hides the Mushroom Indicator
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void HideIndicator(Component sender, object data)
        {
            SetSpriteOpacity(0f);
        }

        /// <summary>
        /// Set the opacity of the SpriteRenderer's sprite color
        /// </summary>
        /// <param name="opacity">A float from 0-1 that describes the opacity of the sprite</param>
        private void SetSpriteOpacity(float opacity)
        {
            // Set the sprite to invisible
            Color newColor = spriteRenderer.color;
            newColor.a = opacity;
            spriteRenderer.color = newColor;
        }
    }

}
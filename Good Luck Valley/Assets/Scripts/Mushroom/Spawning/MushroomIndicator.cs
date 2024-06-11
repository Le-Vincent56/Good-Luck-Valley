using System;
using UnityEngine;

namespace GoodLuckValley.Mushroom
{
    [Serializable]
    public struct FinalSpawnInfo
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public int Angle;

        public FinalSpawnInfo(Vector3 position, Quaternion rotation, int angle)
        {
            Position = position;
            Rotation = rotation;
            Angle = angle;
        }
    }

    public class MushroomIndicator : MonoBehaviour
    {
        #region REFERENCES
        private BoxCollider2D boxCollider;
        private SpriteRenderer spriteRenderer;
        [SerializeField] private Sprite validSprite;
        [SerializeField] private Sprite invalidSprite;
        #endregion

        [Header("Fields")]
        [SerializeField] private FinalSpawnInfo finalSpawnInfo;
        [SerializeField] private float indicatorHeight;

        // Start is called before the first frame update
        void Start()
        {
            // Get the sprite renderer
            boxCollider = GetComponent<BoxCollider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            // Set the sprite to be invisible
            SetSpriteOpacity(0f);

            Bounds bounds = boxCollider.bounds;
            indicatorHeight = bounds.extents.y;
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
            if (data is not ShroomSpawnData) return;

            // Cast the data
            ShroomSpawnData spawnData = (ShroomSpawnData)data;

            // Show Indicator
            SetSpriteOpacity(1f);

            // Rotation of the shroom according to collision data
            Quaternion rotationQuat = Quaternion.AngleAxis(spawnData.Rotation, Vector3.forward);
            transform.rotation = rotationQuat;

            // Get teh spawn point
            Vector2 spawnPoint = spawnData.Point;

            Debug.Log(spawnData.Rotation);

            // Edit spawn point position depending on angle
            switch (spawnData.Rotation)
            {
                case 0:
                    spawnPoint.y += indicatorHeight;
                    break;

                case -180:
                    spawnPoint.y -= indicatorHeight;
                    break;

                case 90:
                    spawnPoint.x -= indicatorHeight;
                    break;

                case -90:
                    spawnPoint.x += indicatorHeight;
                    break;

                case 44:
                    spawnPoint.x -= indicatorHeight * Mathf.Cos(45);
                    spawnPoint.y += indicatorHeight * Mathf.Sin(45);
                    break;

                case 45:
                    spawnPoint.x -= indicatorHeight * Mathf.Cos(45);
                    spawnPoint.y += indicatorHeight * Mathf.Sin(45);
                    break;

                case -45:
                    spawnPoint.x += indicatorHeight * Mathf.Cos(45);
                    spawnPoint.y += indicatorHeight * Mathf.Sin(45);
                    break;

                case -44:
                    spawnPoint.x += indicatorHeight * Mathf.Cos(45);
                    spawnPoint.y += indicatorHeight * Mathf.Sin(45);
                    break;

                case 134:
                    spawnPoint.x -= indicatorHeight * Mathf.Cos(45);
                    spawnPoint.y -= indicatorHeight * Mathf.Sin(45);
                    break;

                case -134:
                    spawnPoint.x += indicatorHeight * Mathf.Cos(45);
                    spawnPoint.y -= indicatorHeight * Mathf.Sin(45);
                    break;

                case 135:
                    spawnPoint.x -= indicatorHeight * Mathf.Cos(45);
                    spawnPoint.y -= indicatorHeight * Mathf.Sin(45);
                    break;

                case -135:
                    spawnPoint.x += indicatorHeight * Mathf.Cos(45);
                    spawnPoint.y -= indicatorHeight * Mathf.Sin(45);
                    break;
            }

            if (spawnData.Valid)
            {
                spriteRenderer.sprite = validSprite;
            } else
            {
                spriteRenderer.sprite = invalidSprite;
            }

            // Set the shroom inidcator position
            transform.position = spawnPoint;

            // Set spawn info
            finalSpawnInfo = new FinalSpawnInfo(spawnPoint, rotationQuat, spawnData.Rotation);
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

        /// <summary>
        /// Send the Final Spawn Info
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void GetFinalSpawnInfo(Component sender, object data)
        {
            // Verify that the correct sender is asking
            if (sender is not MushroomThrow) return;

            // Send the final spawn info
            ((MushroomThrow)sender).SetFinalSpawnInfo(finalSpawnInfo);
        }
    }
}
using GoodLuckValley.Events;
using GoodLuckValley.Events.Mushroom;
using GoodLuckValley.Input;
using GoodLuckValley.Player.Movement;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace GoodLuckValley.Player.Mushroom
{
    public class MushroomSpawner : MonoBehaviour
    {
        [Header("References"), Space()]
        [SerializeField] private GameObject mushroomPrefab;
        [SerializeField] private GameInputReader inputReader;
        private PlayerController playerController;

        private List<MushroomObject> mushrooms;

        [Header("Variables")]
        [SerializeField] private bool canSpawnShroom;
        [SerializeField] private bool canInputMushroom;
        [SerializeField] private float castDistance;
        [SerializeField] private LayerMask shroomableLayers;
        private Vector2 castPosition;

        private EventBinding<SetMushroomInput> onSetMushroomInput;

        private void Awake()
        {
            // Get components
            playerController = GetComponentInParent<PlayerController>();

            // Initialize the list
            mushrooms = new List<MushroomObject>();

            canInputMushroom = false;
        }

        private void OnEnable()
        {
            inputReader.Bounce += Spawn;
            inputReader.Recall += Recall;

            onSetMushroomInput = new EventBinding<SetMushroomInput>(AllowMushroomInput);
            EventBus<SetMushroomInput>.Register(onSetMushroomInput);
        }

        private void OnDisable()
        {
            inputReader.Bounce -= Spawn;
            inputReader.Recall -= Recall;

            EventBus<SetMushroomInput>.Deregister(onSetMushroomInput);
        }

        /// <summary>
        /// Handle mushroom spawning
        /// </summary>
        private void Spawn(bool started)
        {
            // Exit case - if the button is lifted
            if (!started) return;

            // Exit case - if cannot spawn a Mushroom
            if (!canSpawnShroom) return;

            // Exit case - the player cannot input the mushroom
            if(!canInputMushroom) return;

            // Exit case - the Player is crawling
            if (playerController.Crawl.Crawling) return;

            Vector3 spawnPoint;
            float rotation;
            Quaternion rotationQuat;

            // Check if the player is grounded
            if (playerController.Collisions.Grounded)
            {
                // Get the ground hit
                RaycastHit2D groundHit = playerController.Collisions.GroundHit;

                // Set the spawn point to the collision point
                spawnPoint = groundHit.point;
                rotation = (int)Vector2.Angle(groundHit.normal, Vector2.up) * -(int)Mathf.Sign(groundHit.normal.x);
                rotationQuat = Quaternion.AngleAxis(rotation, Vector3.forward);

                // Check if the player is on the slope
                if (playerController.Collisions.IsOnSlope)
                    // Apply the rotation to make it perpendicular to the surface
                    rotationQuat = groundHit.transform.rotation;

                // Create the Mushroom
                CreateMushroom(spawnPoint, rotationQuat);

                return;
            }

            // If not grounded, get the cast position
            castPosition = (Vector2)transform.position;

            // Modify the selected code to use the new method
            RaycastHit2D bounceCast = Physics2D.Raycast(
                castPosition,
                -playerController.Up,
                castDistance,
                shroomableLayers
            );

            // Exit case - nothing was hit
            if (!bounceCast) return;

            // Exit case - there is no detected Tilemap
            if (!bounceCast.collider.TryGetComponent(out Tilemap tilemap)) return;

            // Adjust the hit point to fall within tile boundaries
            Vector2 adjustedPoint = bounceCast.point - (Vector2.up * 0.01f);

            // Get the tile position and verify
            Vector3Int tilePosition = tilemap.WorldToCell(adjustedPoint);

            // Set the spawn point to the bounce cast hit point
            spawnPoint = bounceCast.point;

            // Check if there is a Tile at the position
            if (tilemap.HasTile(tilePosition))
            {
                // Calculate the spawn point at the top of the tile
                Vector3 tileCenter = tilemap.GetCellCenterWorld(tilePosition);
                spawnPoint = tileCenter;
                spawnPoint.y = tileCenter.y + (tilemap.cellSize.y / 2f);
            }

            // Apply the rotation to make it perpendicular to the surface
            rotation = (int)Vector2.Angle(bounceCast.normal, Vector2.up) * -(int)Mathf.Sign(bounceCast.normal.x);
            rotationQuat = Quaternion.AngleAxis(rotation, Vector3.forward);

            // Create the Mushroom
            CreateMushroom(spawnPoint, rotationQuat);
        }

        /// <summary>
        /// Create a Mushroom
        /// </summary>
        private void CreateMushroom(Vector2 spawnPoint, Quaternion rotation)
        {
            // Instantiate the Mushroom
            GameObject shroom = Instantiate(mushroomPrefab, spawnPoint, rotation);

            // Get the height of the Mushroom
            Bounds bounds = shroom.GetComponent<BoxCollider2D>().bounds;
            float mushroomHeight = (bounds.max.y - bounds.min.y) / 2f;

            // Add the Mushroom height to the spawn point
            spawnPoint.y += mushroomHeight;

            // Set the Mushroom's position
            shroom.transform.position = spawnPoint;

            // Initalize the Mushroom Object
            MushroomObject mushroomObj = shroom.GetComponent<MushroomObject>();
            mushroomObj.Initialize();

            // Check if there are Mushrooms stored - this is necessary because there might be 
            // Mushrooms dissipating while another Mushroom is spawning, so this allows them
            // time to dissipate before being deleted
            if(mushrooms.Count > 0)
            {
                // Iterate through each mushroom
                foreach(MushroomObject mushroom in mushrooms)
                {
                    // Dissipate the mushroom
                    mushroom.StartDissipating();
                }
            }

            // Add the Mushroom to the list
            mushrooms.Add(mushroomObj);
        }

        /// <summary>
        /// Recall all stored Mushrooms
        /// </summary>
        private void Recall(bool started)
        {
            // Exit case - if the button is being lifted
            if (!started) return;

            // Exit case - there are no stored Mushrooms
            if (mushrooms.Count <= 0) return;

            // Iterate through each mushroom
            foreach (MushroomObject mushroom in mushrooms)
            {
                // Dissipate the mushroom
                mushroom.StartDissipating();
            }
        }

        private void AllowMushroomInput(SetMushroomInput eventData) => canInputMushroom = eventData.CanInputMushroom;

        /// <summary>
        /// Unlock the Mushroom by allowing the Player to spawn the Mushroom
        /// </summary>
        public bool UnlockMushroom() => canSpawnShroom = true;
    }
}

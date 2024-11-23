using GoodLuckValley.Input;
using GoodLuckValley.Player.Movement;
using UnityEngine;

namespace GoodLuckValley.Player.Mushroom
{
    public class MushroomSpawner : MonoBehaviour
    {
        [Header("References"), Space()]
        [SerializeField] private GameObject mushroomPrefab;
        [SerializeField] private GameInputReader inputReader;
        private PlayerController playerController;
        private BoxCollider2D boxCollider;
        private Vector2 castPoint;

        [Header("Variables")]
        [SerializeField] private float castDistance;
        private Bounds bounds;
        private Vector2 castPosition;

        private void Awake()
        {
            // Get components
            boxCollider = GetComponentInParent<BoxCollider2D>();
            playerController = GetComponentInParent<PlayerController>();

            // Get the raycast point
            bounds = boxCollider.bounds;
            castPoint = new Vector2(bounds.center.x, bounds.min.y);
        }

        private void OnEnable()
        {
            inputReader.Bounce += Spawn;
        }

        private void OnDisable()
        {
            inputReader.Bounce -= Spawn;
        }

        /// <summary>
        /// Handle mushroom spawning
        /// </summary>
        /// <param name="cancelled"></param>
        private void Spawn(bool started)
        {
            // Exit case - if the button is lifted
            if (!started) return;

            castPosition = (Vector2)transform.position;
            castPosition.y = castPoint.y;

            // Raycast beneath the player
            RaycastHit2D bounceCast = Physics2D.Raycast(
                castPosition, 
                -playerController.Up, 
                castDistance, 
                playerController.Stats.CollisionLayers
            );

            // Exit case - nothing was hit
            if (!bounceCast) return;

            // Get the desired spawn point and rotation
            Vector2 spawnPoint = bounceCast.point;
            float rotation = (int)Vector2.Angle(bounceCast.normal, Vector2.up) * -(int)Mathf.Sign(bounceCast.normal.x);
            Quaternion rotationQuat = Quaternion.AngleAxis(rotation, Vector3.forward);

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
            shroom.GetComponent<MushroomObject>().Initialize();
        }

        private void OnDrawGizmos()
        {
            if (castPosition != null && castPosition != Vector2.zero)
                Debug.DrawRay(castPosition, -playerController.Up * castDistance, Color.cyan);
        }
    }
}

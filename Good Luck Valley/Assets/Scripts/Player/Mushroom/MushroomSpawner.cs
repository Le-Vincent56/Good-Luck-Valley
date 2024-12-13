using GoodLuckValley.Architecture.Optionals;
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

        public Optional<MushroomObject> Mushroom { get; private set; } = Optional<MushroomObject>.None();

        [Header("Variables")]
        [SerializeField] private float castDistance;
        [SerializeField] private LayerMask nonShroomableLayers;
        private Vector2 castPosition;

        private void Awake()
        {
            // Get components
            playerController = GetComponentInParent<PlayerController>();
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

            // Raycast beneath the player to check for non-shroomable layers
            RaycastHit2D checkCast = Physics2D.Raycast(
                castPosition,
                -playerController.Up,
                castDistance,
                nonShroomableLayers
            );

            // Exit case - has hit a non-shroomable layer
            if (checkCast) return;

            // Raycast beneath the player to check for shroomable layers
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
            MushroomObject mushroomObj = shroom.GetComponent<MushroomObject>();
            mushroomObj.Initialize();

            // Dissipate any current Mushrooms
            Mushroom.Match(
                onValue: mushroom => { mushroom.StartDissipating(); return 0; }, // Start dissipating the mushroom
                onNoValue: () => { return 0; } // Do nothing
            );

            // Set the Mushroom
            Mushroom = Optional<MushroomObject>.Some(mushroomObj);
        }
    }
}

using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Player.Movement;
using UnityEngine;

namespace GoodLuckValley.Scenes
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class SceneTransporter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SceneGroupData sceneGroupData;
        [SerializeField] private SceneLoader sceneLoader;
        [SerializeField, HideInInspector] private int sceneIndexToLoad;

        [Header("Other")]
        [SerializeField] private int moveDirection;

        public int SceneIndexToLoad { get => sceneIndexToLoad; set => sceneIndexToLoad = value; }

        public SceneGroupData SceneGroupData { get => sceneGroupData; }

        private void Start()
        {
            // Retrieve the SceneLoader service
            sceneLoader = ServiceLocator.Global.Get<SceneLoader>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Exit case - the collision is not the Player
            if (!collision.TryGetComponent(out PlayerController controller)) return;

            // Remove manual move
            controller.ManualMove = false;
            controller.ForcedMoveDirection = moveDirection;

            // Start changing the scene group
            sceneLoader.ChangeSceneGroup(sceneIndexToLoad, moveDirection);
        }
    }
}

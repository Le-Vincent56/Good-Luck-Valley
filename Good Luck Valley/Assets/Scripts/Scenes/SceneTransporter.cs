using GoodLuckValley.Player.Movement;
using UnityEngine;

namespace GoodLuckValley.Scenes
{
    public enum SceneGate
    {
        Entrance,
        Exit
    }

    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class SceneTransporter : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private SceneGroupData sceneGroupData;
        [SerializeField, HideInInspector] private int sceneIndexToLoad;

        [Header("Other")]
        [SerializeField] private int moveDirection;
        [SerializeField] private SceneGate toGate;

        public int SceneIndexToLoad { get => sceneIndexToLoad; set => sceneIndexToLoad = value; }

        public SceneGroupData SceneGroupData { get => sceneGroupData; }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Exit case - the collision is not the Player
            if (!collision.TryGetComponent(out PlayerController controller)) return;

            // Exit case - already being forced to move
            if(controller.ForcedMove) return;

            // Remove manual move
            controller.ForcedMove = true;
            controller.ForcedMoveDirection = moveDirection;

            // Start changing the scene group
            SceneLoader.Instance.ChangeSceneGroupLevel(sceneIndexToLoad, toGate, moveDirection);
        }
    }
}

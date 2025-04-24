using GoodLuckValley.Player.Movement;
using GoodLuckValley.World.Triggers;
using UnityEngine;

namespace GoodLuckValley.Scenes
{
    public enum SceneGate
    {
        Entrance,
        Exit
    }

    public class SceneTransporter : EnterExitTrigger
    {
        [Header("References")]
        [SerializeField] private SceneGroupData sceneGroupData;
        [SerializeField, HideInInspector] private int sceneIndexToLoad;

        [Header("Other")]
        [SerializeField] private bool showLoadingSymbol = true;
        [SerializeField] private int moveDirection;
        [SerializeField] private SceneGate toGate;

        public int SceneIndexToLoad { get => sceneIndexToLoad; set => sceneIndexToLoad = value; }

        public SceneGroupData SceneGroupData { get => sceneGroupData; }

        public override void OnEnter(PlayerController controller)
        {
            // Exit case - already being forced to move
            if (controller.ForcedMove && controller.ForcedMoveDirection != 0) return;

            // Remove manual move
            controller.ForcedMove = true;
            controller.ForcedMoveDirection = moveDirection;

            // Set loading from gate
            sceneLoader.LoadingFromGate = true;

            // Start changing the scene group
            sceneLoader.ChangeSceneGroupLevel(sceneIndexToLoad, toGate, showLoadingSymbol, moveDirection);
        }

        public override void OnExit(PlayerController controller) { /* Noop */ }
    }
}

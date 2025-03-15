using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.Input;
using GoodLuckValley.Scenes;
using UnityEngine;

namespace GoodLuckValley.World.Cinematics
{
    public class CallToAction : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameInputReader inputReader;
        [SerializeField] private SceneLoader sceneLoader;

        [Header("Fields")]
        [SerializeField] private bool active;

        private void Awake()
        {
            // Get the Scene Loader
            sceneLoader = ServiceLocator.Global.Get<SceneLoader>();

            // Set active to false
            active = false;
        }

        private void OnEnable()
        {
            inputReader.Enable();
            inputReader.ContinueToMain += ContinueToMain;
        }

        private void OnDisable()
        {
            inputReader.ContinueToMain -= ContinueToMain;
            inputReader.Disable();
        }

        /// <summary>
        /// Continue to the Main Menu
        /// </summary>
        private void ContinueToMain(bool started)
        {
            // Exit case - not active
            if (!active) return;

            // Exit case - the button was pressed down
            if (started) return;

            // Change the scene to the main menu
            sceneLoader.ChangeSceneGroupSystem(0);

            // Disable the input reader
            inputReader.Disable();
        }

        /// <summary>
        /// Allow the Player to return to the Main Menu by activating
        /// </summary>
        public void Activate() => active = true;
    }
}

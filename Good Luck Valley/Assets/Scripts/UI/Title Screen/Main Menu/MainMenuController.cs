using GoodLuckValley.UI.TitleScreen.Start;
using UnityEngine;

namespace GoodLuckValley.UI.TitleScreen.Main
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private TitleScreenController controller;
        [SerializeField] private StartMenuController startMenuController;

        private void Awake()
        {
            // Get components
            controller = GetComponentInParent<TitleScreenController>();
        }

        /// <summary>
        /// Activate the start menu
        /// </summary>
        public void StartGame()
        {
            controller.SetState(controller.START);

            startMenuController.SetSlotData();
        }

        /// <summary>
        /// Enter the settings menu
        /// </summary>
        public void EnterSettings() => controller.SetState(controller.SETTINGS);

        /// <summary>
        /// Quit the game
        /// </summary>
        public void QuitGame() => Application.Quit();
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.UI.TitleScreen.Main
{
    public class MainMenuController : MonoBehaviour
    {
        [SerializeField] private TitleScreenController controller;

        private void Awake()
        {
            // Get components
            controller = GetComponentInParent<TitleScreenController>();
        }

        /// <summary>
        /// Activate the start menu
        /// </summary>
        public void StartGame() => controller.SetState(controller.START);

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
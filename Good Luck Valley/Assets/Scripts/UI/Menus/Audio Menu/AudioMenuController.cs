using GoodLuckValley.UI.Menus.Main;
using GoodLuckValley.UI.Menus.Persistence;
using UnityEngine;

namespace GoodLuckValley.UI.Menus.Audio
{
    public class AudioMenuController : MonoBehaviour, IMenuController
    {
        private MainMenuController mainMenuController;
        private AudioSaveHandler saveHandler;

        private void Awake()
        {
            // Get components
            mainMenuController = GetComponentInParent<MainMenuController>();
            saveHandler = GetComponent<AudioSaveHandler>();
        }

        /// <summary>
        /// Leave the Audio menu
        /// </summary>
        public void Back()
        {
            // Save data
            saveHandler.SaveData();

            // Set the settings state
            mainMenuController.SetState(mainMenuController.SETTINGS);
        }
    }
}

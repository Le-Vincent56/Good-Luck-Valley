using GoodLuckValley.UI.Menus.Main;
using GoodLuckValley.UI.Menus.Persistence;
using UnityEngine;

namespace GoodLuckValley.UI.Menus.Video
{
    public class VideoMenuController : MonoBehaviour, IMenuController
    {
        private MainMenuController mainMenuController;
        private VideoSaveHandler saveHandler;

        private void Awake()
        {
            // Get components
            mainMenuController = GetComponentInParent<MainMenuController>();
            saveHandler = GetComponent<VideoSaveHandler>();
        }

        /// <summary>
        /// Leave the Video menu
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

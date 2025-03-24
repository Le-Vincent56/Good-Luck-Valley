using GoodLuckValley.UI.Menus.Pause;
using GoodLuckValley.UI.Menus.Persistence;
using UnityEngine;

namespace GoodLuckValley.UI.Menus.Video
{
    public class VideoPauseController : MonoBehaviour, IMenuController
    {
        private PauseMenuController pauseMenuController;
        private VideoSaveHandler saveHandler;

        private void Awake()
        {
            // Get components
            pauseMenuController = GetComponentInParent<PauseMenuController>();
            saveHandler = GetComponent<VideoSaveHandler>();
        }

        /// <summary>
        /// Leave the Video menu
        /// </summary>
        public void Back()
        {
            // Save Video data
            saveHandler.SaveData();

            // Set the settings state
            pauseMenuController.SetState(pauseMenuController.SETTINGS);
        }
    }
}

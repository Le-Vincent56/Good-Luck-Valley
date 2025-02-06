using GoodLuckValley.UI.Menus.Pause;
using GoodLuckValley.UI.Menus.Persistence;
using UnityEngine;

namespace GoodLuckValley.UI.Menus.Audio
{
    public class AudioPauseController : MonoBehaviour, IMenuController
    {
        private PauseMenuController pauseMenuController;
        private AudioSaveHandler saveHandler;

        private void Awake()
        {
            pauseMenuController = GetComponentInParent<PauseMenuController>();
            saveHandler = GetComponent<AudioSaveHandler>();
        }

        /// <summary>
        /// Leave the Audio menu
        /// </summary>
        public void Back()
        {
            // Save Audio data
            saveHandler.SaveData();

            // Set the settings state
            pauseMenuController.SetState(pauseMenuController.SETTINGS);

            Debug.Log($"Setting State: {pauseMenuController.SETTINGS}");
        }
    }
}

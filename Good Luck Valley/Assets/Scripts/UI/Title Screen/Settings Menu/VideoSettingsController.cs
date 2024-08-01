using GoodLuckValley.UI.Settings.Video;
using UnityEngine;

namespace GoodLuckValley.UI.TitleScreen.Settings.Video
{
    public class VideoSettingsController : SettingsController
    {
        [Header("References")]
        [SerializeField] private VideoSaveHandler videoSaveHandler;
        [SerializeField] private BrightnessSlider brightnessSlider;

        [Header("Fields")]
        private const int stateNum = 5;

        protected override void Awake()
        {
            base.Awake();

            // Get components
            videoSaveHandler = GetComponent<VideoSaveHandler>();

            // Initialize elements
            brightnessSlider.Init();
        }

        /// <summary>
        /// Handle going back to the main settings menu
        /// </summary>
        public void BackToSettings()
        {
            // Save settings data
            videoSaveHandler.SaveData();

            // Set the settings state
            controller.SetState(controller.SETTINGS);
        }

        /// <summary>
        /// Handle back input
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void BackInput(Component sender, object data)
        {
            // Verify that the correct data was sent
            if (data is not int) return;

            // Cast and compare data
            if ((int)data == stateNum)
            {
                BackToSettings();
            }
        }

        /// <summary>
        /// Reset the video settings to their defaults
        /// </summary>
        public void ResetSettings()
        {
            brightnessSlider.LoadBrightness(50f);
        }
    }
}
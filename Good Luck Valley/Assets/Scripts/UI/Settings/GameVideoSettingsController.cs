using UnityEngine;

namespace GoodLuckValley.UI.Settings.Video
{
    public class GameVideoSettingsController : MonoBehaviour
    {
        [Header("Wwise Events")]
        [SerializeField] private AK.Wwise.Event playButtonReset;

        [Header("References")]
        [SerializeField] private GameSettingsMenu controller;
        [SerializeField] private VideoSaveHandler videoSaveHandler;
        [SerializeField] private BrightnessSlider brightnessSlider;

        [Header("Fields")]
        private const int stateNum = 5;

        private void Awake()
        {
            // Initialize
            Init(this, null);
        }

        /// <summary>
        /// Handle going back to the main settings menu
        /// </summary>
        public void BackToSettings()
        {
            // Save settings data
            videoSaveHandler.SaveData();

            // Set the settings state
            controller.SetState(controller.MAIN);
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
            // Set brightness to 50
            brightnessSlider.LoadBrightness(50f);

            // Play the reset button sound
            playButtonReset.Post(gameObject);
        }

        /// <summary>
        /// Initialize the menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void Init(Component sender, object data)
        {
            // Get components
            if(controller == null)
                controller = GetComponentInParent<GameSettingsMenu>();

            if(videoSaveHandler == null)
                videoSaveHandler = GetComponent<VideoSaveHandler>();

            // Initialize elements
            brightnessSlider.Init();
        }
    }
}
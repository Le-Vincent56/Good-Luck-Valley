using GoodLuckValley.Persistence;
using GoodLuckValley.UI.Settings.Audio;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.UI.TitleScreen.Settings.Audio
{
    public class AudioSettingsController : SettingsController
    {
        [Header("Wwise Events")]
        [SerializeField] private AK.Wwise.Event playButtonReset;

        [Header("References")]
        [SerializeField] private List<AudioBusSlider> audioBusSliders = new List<AudioBusSlider>();
        [SerializeField] private AudioSaveHandler audioSaveHandler;

        [Header("Fields")]
        private const int stateNum = 4;

        protected override void Awake()
        {
            base.Awake();

            // Get components
            audioSaveHandler = GetComponent<AudioSaveHandler>();

            // Initialize every slider
            foreach (AudioBusSlider slider in audioBusSliders)
            {
                slider.Init();
            }
        }

        /// <summary>
        /// Handle going back to the main settings menu
        /// </summary>
        public void BackToSettings()
        {
            // Save settings
            audioSaveHandler.SaveData();

            // Set the state
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
        /// Reset the audio settings to their defaults
        /// </summary>
        public void ResetSettings()
        {
            // Set each volume slider to 100
            foreach(AudioBusSlider audioBusSlider in audioBusSliders)
            {
                audioBusSlider.LoadVolume(100f);
            }

            // Play the reset button sound
            playButtonReset.Post(gameObject);
        }
    }
}
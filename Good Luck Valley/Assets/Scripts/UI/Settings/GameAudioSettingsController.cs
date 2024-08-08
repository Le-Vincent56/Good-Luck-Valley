using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.UI.Settings.Audio
{
    public class GameAudioSettingsController : MonoBehaviour
    {
        [Header("Wwise Events")]
        [SerializeField] private AK.Wwise.Event playButtonReset;

        [Header("References")]
        [SerializeField] private GameSettingsMenu controller;
        [SerializeField] private List<AudioBusSlider> audioBusSliders = new List<AudioBusSlider>();
        [SerializeField] private AudioSaveHandler audioSaveHandler;

        [Header("Fields")]
        private const int stateNum = 4;

        private void Start()
        {
            // Initialize
            Init(this, null);
        }

        /// <summary>
        /// Handle going back to the main settings menu
        /// </summary>
        public void BackToSettings()
        {
            // Save settings
            audioSaveHandler.SaveData();

            // Set the state
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

            if(audioSaveHandler == null)
                audioSaveHandler = GetComponent<AudioSaveHandler>();

            // Initialize every slider
            foreach (AudioBusSlider slider in audioBusSliders)
            {
                slider.Init();
            }
        }
    }
}
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.UI.TitleScreen.Settings.Audio
{
    public class AudioSettingsController : SettingsController
    {
        [Header("Referneces")]
        [SerializeField] private List<AudioBusSlider> audioBusSliders = new List<AudioBusSlider>();

        [Header("Fields")]
        private const int stateNum = 4;

        private void Start()
        {
            // Initialize every slider
            foreach(AudioBusSlider slider in audioBusSliders)
            {
                slider.Init(this);
            }
        }

        public void BackToSettings() => controller.SetState(controller.SETTINGS);

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

        public void ResetSettings() { }
    }
}
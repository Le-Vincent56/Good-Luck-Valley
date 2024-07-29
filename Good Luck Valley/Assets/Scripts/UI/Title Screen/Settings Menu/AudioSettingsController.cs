using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.UI.TitleScreen.Settings.Audio
{
    public class AudioSettingsController : SettingsController
    {
        [SerializeField] private List<AudioBusSlider> audioBusSliders = new List<AudioBusSlider>();

        private void Start()
        {
            // Initialize every slider
            foreach(AudioBusSlider slider in audioBusSliders)
            {
                slider.Init(this);
            }
        }

        public void BackToSettings() => controller.SetState(controller.SETTINGS);
        public void ResetSettings() { }
    }
}
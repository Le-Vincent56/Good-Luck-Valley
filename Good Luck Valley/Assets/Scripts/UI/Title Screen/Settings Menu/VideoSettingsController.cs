using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace GoodLuckValley.UI.TitleScreen.Settings
{
    public class VideoSettingsController : SettingsController
    {
        [Header("References")]
        [SerializeField] private Slider brightnessSlider;
        [SerializeField] private PostProcessVolume postProcessVolume;
        private ColorAdjustments colorAdjustments;

        [Header("Fields")]
        private const int stateNum = 5;

        //private void Start()
        //{
        //    if(postProcessVolume.profile.TryGetSettings(out colorAdjustments))
        //    {
        //        // TODO: Saved settings
        //        // Set the slider's value to the saved brightness or default value
        //        //float savedBrightness = PlayerPrefs.GetFloat("Brightness", 0);
        //        //brightnessSlider.value = savedBrightness;
        //        //UpdateBrightness(savedBrightness);

        //        // Add a listener to the brightness slider
        //        brightnessSlider.onValueChanged.AddListener(UpdateBrightness);
        //    }
        //}

        //private void UpdateBrightness(float value)
        //{
        //    if(colorAdjustments != null)
        //    {
        //        colorAdjustments.postExposure.value = value;

        //        // TODO: Save the brightness
        //    }
        //}

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
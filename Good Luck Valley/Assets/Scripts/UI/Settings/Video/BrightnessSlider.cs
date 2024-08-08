using GoodLuckValley.UI.Elements;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Settings.Video
{
    public class BrightnessSlider : MonoBehaviour
    {
        [Header("Wwise Events")]
        [SerializeField] private AK.Wwise.Event playSlider;

        [Header("References")]
        [SerializeField] private SliderInput brightnessSlider;
        [SerializeField] private Text displayText;
        [SerializeField] private VolumeProfile postProcessVolume;
        [SerializeField] private ColorAdjustments colorAdjustments;

        [Header("Fields")]
        [SerializeField] private bool loaded;

        /// <summary>
        /// Initialize the brightness slider
        /// </summary>
        public void Init()
        {
            // Get the lsider component
            brightnessSlider = GetComponent<SliderInput>();

            // Try to get the color adjustments
            if (postProcessVolume.TryGet(out colorAdjustments))
            {
                // TODO: Load data

                brightnessSlider.onValueChanged.AddListener(UpdateBrightness);
            }

            // Set loaded to false
            loaded = false;
        }

        /// <summary>
        /// Update the brightness of the game
        /// </summary>
        /// <param name="value"></param>
        public void UpdateBrightness(float value)
        {
            // Exit case - the color adjustments component was not able to be retrieved
            if (colorAdjustments == null) return;

            // Edit the value
            float editedValue = Mathf.Clamp((value - 50) / 100f, -0.5f, 0.5f);

            // Set the exposure value
            colorAdjustments.postExposure.value = editedValue;

            // Update the display text
            displayText.text = $"{(int)value}";

            // Check if the game object is active and data is already loaded
            if (gameObject.activeSelf && loaded)
                // Play the slider sound
                playSlider.Post(gameObject);
        }

        public void LoadBrightness(float value)
        {
            // Update the brightness
            UpdateBrightness(value);

            // Update the slider
            brightnessSlider.SetValueWithoutNotify(value);

            // Set loaded to true
            loaded = true;
        }

        /// <summary>
        /// Get the current brightness value from the brightness slider
        /// </summary>
        /// <returns>The current brightness value</returns>
        public float GetBrightness() => brightnessSlider.value;
    }
}
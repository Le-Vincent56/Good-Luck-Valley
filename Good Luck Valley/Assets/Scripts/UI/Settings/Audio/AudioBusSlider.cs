using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Settings.Audio
{
    public class AudioBusSlider : MonoBehaviour
    {
        [SerializeField] private Slider controlSlider;
        [SerializeField] private Text displayText;
        [SerializeField] private AK.Wwise.RTPC volumeRTPC;

        /// <summary>
        /// Initialize the Audio Bus Slider
        /// </summary>
        public void Init()
        {
            // Get slider if not set
            if (controlSlider == null)
                controlSlider = GetComponent<Slider>();

            // Add listeners to the sliders
            controlSlider.onValueChanged.AddListener(SetVolume);
        }

        /// <summary>
        /// Set the value for the Audio Bus Slider
        /// </summary>
        /// <param name="value">The value to set</param>
        public void SetVolume(float value)
        {
            AkSoundEngine.SetRTPCValue(volumeRTPC.Id, value);

            // Set the text
            displayText.text = ((int)value).ToString();
        }

        public void LoadVolume(float value)
        {
            // Set the volume
            SetVolume(value);

            // Change the slider control
            controlSlider.SetValueWithoutNotify(value);
        }

        /// <summary>
        /// Get the current volume value of the Audio Bus Slider
        /// </summary>
        /// <returns>The current volume value of the Audio Bus Slider</returns>
        public float GetVolume() => controlSlider.value;
    }
}
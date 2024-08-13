using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Settings.Audio
{
    public class AudioBusSlider : MonoBehaviour
    {
        [Header("Wwise Events")]
        [SerializeField] private AK.Wwise.RTPC volumeRTPC;
        [SerializeField] private AK.Wwise.Event playSlider;

        [Header("References")]
        [SerializeField] private Slider controlSlider;
        [SerializeField] private Text displayText;

        [Header("Fields")]
        [SerializeField] private bool loaded;

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

            // Set loaded to false
            loaded = false;
        }

        /// <summary>
        /// Set the value for the Audio Bus Slider
        /// </summary>
        /// <param name="value">The value to set</param>
        public void SetVolume(float value)
        {
            // Set RTPC values
            AkSoundEngine.SetRTPCValue(volumeRTPC.Id, value);

            // Set the text
            displayText.text = ((int)value).ToString();

            // Check if the game object is active and data is already loaded
            if(gameObject.activeSelf && loaded)
                // Play slider sound
                playSlider.Post(gameObject);
        }

        public void LoadVolume(float value)
        {
            // Set the volume
            SetVolume(value);

            // Change the slider control
            controlSlider.SetValueWithoutNotify(value);

            // Set loaded to true
            loaded = true;
        }

        /// <summary>
        /// Get the current volume value of the Audio Bus Slider
        /// </summary>
        /// <returns>The current volume value of the Audio Bus Slider</returns>
        public float GetVolume() => controlSlider.value;
    }
}
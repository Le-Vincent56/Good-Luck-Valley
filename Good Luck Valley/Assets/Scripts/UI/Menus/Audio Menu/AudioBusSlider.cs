using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Menus.Audio
{
    public class AudioBusSlider : MonoBehaviour
    {
        [Header("Wwise Events")]
        [SerializeField] private AK.Wwise.RTPC volumeRTPC;
        [SerializeField] private AK.Wwise.Event playSlider;

        [Header("References")]
        [SerializeField] private SliderInput controlSlider;
        [SerializeField] private Text displayText;

        [Header("Fields")]
        [SerializeField] private bool loaded;

        /// <summary>
        /// Initialize the Audio Bus Slider
        /// </summary>
        private void Awake()
        {
            // Get components
            controlSlider = GetComponent<SliderInput>();

            // Add listeners to the sliders
            controlSlider.onValueChanged.AddListener(SetVolume);
        }

        /// <summary>
        /// Set the value for the Audio Bus Slider
        /// </summary>
        public void SetVolume(float value)
        {
            // Set RTPC values
            volumeRTPC.SetGlobalValue(value);

            // Set the text
            displayText.text = ((int)value).ToString();

            // Check if the game object is active and data is already loaded
            if (gameObject.activeSelf && loaded)
                // Play slider sound
                playSlider.Post(gameObject);
        }

        /// <summary>
        /// Load the value for the Audio Bus Slider
        /// </summary>
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
        public float GetVolume() => controlSlider.value;
    }
}

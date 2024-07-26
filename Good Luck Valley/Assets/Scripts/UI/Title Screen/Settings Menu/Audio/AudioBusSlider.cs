using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.TitleScreen.Settings.Audio
{
    public class AudioBusSlider : MonoBehaviour
    {
        [SerializeField] private AudioSettingsController controller;
        [SerializeField] private Slider controlSlider;
        [SerializeField] private Text displayText;
        [SerializeField] private AK.Wwise.RTPC volumeRTPC;

        public void Init(AudioSettingsController controller)
        {
            // Get slider if not set
            if (controlSlider == null)
                controlSlider = GetComponent<Slider>();

            this.controller = controller;

            // Add listeners to the sliders
            controlSlider.onValueChanged.AddListener(SetVolume);
        }

        private void SetVolume(float value)
        {
            AkSoundEngine.SetRTPCValue(volumeRTPC.Id, value);

            // Set the volume for the RTPC
            //volumeRTPC.SetValue(controller.gameObject, value);

            // Set the text
            displayText.text = ((int)value).ToString();
        }
    }
}
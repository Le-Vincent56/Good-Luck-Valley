using GoodLuckValley.Persistence;
using GoodLuckValley.UI.Menus.Audio;
using UnityEngine;

namespace GoodLuckValley.UI.Menus.Persistence
{
    public class AudioSaveHandler : MonoBehaviour, IBind<AudioData>
    {
        [Header("References")]
        [SerializeField] private AudioData data;
        [SerializeField] private AudioBusSlider masterSlider;
        [SerializeField] private AudioBusSlider musicSlider;
        [SerializeField] private AudioBusSlider sfxSlider;
        [SerializeField] private AudioBusSlider ambienceSlider;

        [field: SerializeField] public SerializableGuid ID { get; set; } = new SerializableGuid(717968785, 1113349830, 333676446, 1625166900);

        /// <summary>
        /// Save the Audio data
        /// </summary>
        public void SaveData()
        {
            // Set volume data
            data.MasterVolume = masterSlider.GetVolume();
            data.MusicVolume = musicSlider.GetVolume();
            data.SFXVolume = sfxSlider.GetVolume();
            data.AmbienceVolume = ambienceSlider.GetVolume();

            // Save the settings
            SaveLoadSystem.Instance.SaveSettings();
        }

        /// <summary>
        /// Bind the Audio data
        /// </summary>
        public void Bind(AudioData data)
        {
            // Bind the data
            this.data = data;
            this.data.ID = data.ID;

            // Load audio data
            masterSlider.LoadVolume(data.MasterVolume);
            musicSlider.LoadVolume(data.MusicVolume);
            sfxSlider.LoadVolume(data.SFXVolume);
            ambienceSlider.LoadVolume(data.AmbienceVolume);
        }

        /// <summary>
        /// Reset the audio data
        /// </summary>
        public void ResetData()
        {
            // Get the reset data presets
            AudioData resetData = new AudioData();
            
            // Load the slider volumes
            masterSlider.LoadVolume(resetData.MasterVolume);
            musicSlider.LoadVolume(resetData.MusicVolume);
            sfxSlider.LoadVolume(resetData.SFXVolume);
            ambienceSlider.LoadVolume(resetData.AmbienceVolume);
        }
    }
}

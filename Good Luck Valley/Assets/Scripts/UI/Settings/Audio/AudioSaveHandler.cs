using GoodLuckValley.Persistence;
using GoodLuckValley.Player.Input;
using GoodLuckValley.UI.TitleScreen.Settings.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GoodLuckValley.UI.Settings.Audio
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

        public void SaveData()
        {
            // Save volume data
            data.masterVolume = masterSlider.GetVolume();
            data.musicVolume = musicSlider.GetVolume();
            data.sfxVolume = sfxSlider.GetVolume();
            data.ambienceVolume = ambienceSlider.GetVolume();

            SaveLoadSystem.Instance.SaveSettings();
        }

        public void Bind(AudioData data, bool applyData = true)
        {
            this.data = data;
            this.data.ID = data.ID;

            // Set slider data
            masterSlider.LoadVolume(data.masterVolume);
            musicSlider.LoadVolume(data.musicVolume);
            sfxSlider.LoadVolume(data.sfxVolume);
            ambienceSlider.LoadVolume(data.ambienceVolume);
        }
    }
}
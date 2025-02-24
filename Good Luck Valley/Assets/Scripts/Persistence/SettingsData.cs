using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GoodLuckValley.Persistence
{
    [Serializable]
    public class SettingsData : Data
    {
        public AudioData Audio;
        public VideoData Video;
        public ControlsData Controls;
    }

    [Serializable]
    public class AudioData : ISaveable
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        public float MasterVolume;
        public float MusicVolume;
        public float AmbienceVolume;
        public float SFXVolume;

        public AudioData()
        {
            MasterVolume = 100f;
            MusicVolume = 100f;
            AmbienceVolume = 100f;
            SFXVolume = 100f;
        }
    }

    [Serializable]
    public class VideoData : ISaveable
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        public float Brightness;

        public VideoData()
        {
            Brightness = 50f;
        }
    }

    [Serializable]
    public class ControlsData : ISaveable
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        public string Bindings;

        public ControlsData(InputActionAsset inputActions)
        {
            inputActions.RemoveAllBindingOverrides();
            Bindings = inputActions.SaveBindingOverridesAsJson();
        }

        public ControlsData() { }
    }
}

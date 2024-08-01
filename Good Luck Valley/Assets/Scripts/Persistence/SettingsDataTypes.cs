using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GoodLuckValley.Persistence
{
    public class SettingsData
    {
        public string Name;
        public AudioData Audio;
        public VideoData Video;
        public ControlsData Controls;
    }

    [Serializable]
    public class ControlsData : ISaveable
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        public string bindings;

        public ControlsData(InputActionAsset inputActions)
        {
            inputActions.RemoveAllBindingOverrides();
            bindings = inputActions.SaveBindingOverridesAsJson();
        }

        public ControlsData() { }
    }

    [Serializable]
    public class AudioData : ISaveable
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        public float masterVolume;
        public float musicVolume;
        public float ambienceVolume;
        public float sfxVolume;

        public AudioData()
        {
            masterVolume = 100f;
            musicVolume = 100f;
            ambienceVolume = 100f;
            sfxVolume = 100f;
        }
    }

    [Serializable]
    public class VideoData : ISaveable
    {
        [field: SerializeField] public SerializableGuid ID { get; set; }
        public float brightness;

        public VideoData()
        {
            brightness = 50f;
        }
    }
}
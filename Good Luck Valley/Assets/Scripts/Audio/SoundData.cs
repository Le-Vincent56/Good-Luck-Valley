using System;
using UnityEngine;
using UnityEngine.Audio;

namespace GoodLuckValley.Audio.Sound
{
    [CreateAssetMenu(fileName = "SoundData")]
    public class SoundData : ScriptableObject
    {
        public AudioClip[] clips;
        public AudioMixerGroup mixerGroup;
        public bool loop;
        public bool playOnAwake;
        public bool frequentSound;
    }
}
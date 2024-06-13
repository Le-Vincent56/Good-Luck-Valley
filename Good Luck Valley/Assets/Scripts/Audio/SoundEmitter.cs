using GoodLuckValley.Patterns.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Audio.Sound
{
    [RequireComponent(typeof(AudioSource))]
    public class SoundEmitter : MonoBehaviour
    {
        private AudioSource audioSource;
        private Coroutine playingCoroutine;

        public SoundData Data { get; private set; }
        public LinkedListNode<SoundEmitter> Node { get; set; }

        private void Awake()
        {
            // Get the audio source component
            audioSource = gameObject.GetOrAdd<AudioSource>();
        }

        /// <summary>
        /// Initialize the SoundEmitter
        /// </summary>
        /// <param name="data">The SoundData to set</param>
        public void Initialize(SoundData data)
        {
            // Set the SoundData
            Data = data;

            // Set the AudioSource data from the SoundData data
            audioSource.clip = data.clips[Random.Range(0, data.clips.Length)];
            audioSource.outputAudioMixerGroup = data.mixerGroup;
            audioSource.loop = data.loop;
            audioSource.playOnAwake = data.playOnAwake;
            audioSource.volume = data.volume;
            audioSource.spatialBlend = data.spatialBlend;
        }

        /// <summary>
        /// Play the SoundEmitter
        /// </summary>
        public void Play()
        {
            // Check if the audio is already playing
            if (playingCoroutine != null)
                // If so, stop it from playing
                StopCoroutine(playingCoroutine);

            // Play the sound
            audioSource.Play();

            // Start the playing coroutine
            playingCoroutine = StartCoroutine(WaitForSoundToEnd());
        }

        /// <summary>
        /// Coroutine to wait for a sound to end
        /// </summary>
        /// <returns></returns>
        private IEnumerator WaitForSoundToEnd()
        {
            // Wait for the audio source to stop playing
            yield return new WaitWhile(() => audioSource.isPlaying);

            // Return this SoundEmitter to the pool
            SoundManager.Instance.ReturnToPool(this);
        }

        /// <summary>
        /// Stop the SoundEmitter
        /// </summary>
        public void Stop()
        {
            // Check if the audio is already playing
            if(playingCoroutine != null)
            {
                // Stop playing the coroutine and set it to null
                StopCoroutine(playingCoroutine);
                playingCoroutine = null;
            }

            // Stop the audio source
            audioSource.Stop();

            // Return the SoundEmitter to the pool
            SoundManager.Instance.ReturnToPool(this);
        }

        /// <summary>
        /// Play the sound with a randomized pitch
        /// </summary>
        /// <param name="min">The minimum range of the pitch from the original</param>
        /// <param name="max">The maximum range of the pitch from the original</param>
        public void WithRandomPitch(float min = -0.05f, float max = 0.05f)
        {
            audioSource.pitch += Random.Range(min, max);
        }
    }
}
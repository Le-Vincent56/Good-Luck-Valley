using GoodLuckValley.Architecture.Singletons;
using GoodLuckValley.Persistence;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Audio
{
    public class MusicManager : PersistentSingleton<MusicManager>
    {
        [Header("Wwise Events")]
        [SerializeField] private AK.Wwise.Event startMusicEvent;
        [SerializeField] private AK.Wwise.Event stopMusicEvent;
        [SerializeField] private AK.Wwise.Event pauseMusicEvent;
        [SerializeField] private AK.Wwise.Event resumeMusicEvent;

        [Header("Fields")]
        private Dictionary<uint, uint> musicStates;
        private Dictionary<uint, string> stateNames;
        [SerializeField] private bool isPlaying;

        [SerializeField] private List<AK.Wwise.State> menuStates = new List<AK.Wwise.State>();

        [field: SerializeField] public SerializableGuid ID { get; set; } = new SerializableGuid(3161503128, 1247231471, 712909453, 4222699218);

        /// <summary>
        /// Play the music event
        /// </summary>
        public void Play()
        {
            // Exit case - if the music is already playing
            if (isPlaying) return;

            // Post the play music event
            startMusicEvent.Post(gameObject);

            // Set to playing
            isPlaying = true;
        }

        /// <summary>
        /// Stop the music event
        /// </summary>
        public void Stop()
        {
            // Exit case - if the music is not playing
            if(!isPlaying) return;

            // Post the stop music event
            stopMusicEvent.Post(gameObject);

            // Set to not playing
            isPlaying = false;
        }

        /// <summary>
        /// Pause the music event
        /// </summary>
        public void Pause()
        {
            // Exit case - if the music is not playing
            if (!isPlaying) return;

            // Post the pause music event
            pauseMusicEvent.Post(gameObject);

            // Set to not playing
            isPlaying = false;
        }

        /// <summary>
        /// Resume the music event
        /// </summary>
        public void Resume()
        {
            // Exit case - if the music is already playing
            if (isPlaying) return;

            // Post the resume music event
            resumeMusicEvent.Post(gameObject);

            // Set to playing
            isPlaying = true;
        }

        /// <summary>
        /// Switch music event states
        /// </summary>
        public void SetState(AK.Wwise.State state)
        {
            // Set the state value
            state.SetValue();
        }

        public void Restart()
        {
            // Stop the music
            Stop();

            // Play the music
            Play();
        }

        public void SetStates(List<AK.Wwise.State> states)
        {
            // Iterate through each state
            foreach (AK.Wwise.State state in states)
            {
                // Set the state
                state.SetValue();
            }
        }
    }
}

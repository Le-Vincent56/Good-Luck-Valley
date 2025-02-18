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
        public void Stop() => stopMusicEvent.Post(gameObject);

        /// <summary>
        /// Pause the music event
        /// </summary>
        public void Pause() => pauseMusicEvent.Post(gameObject);

        /// <summary>
        /// Resume the music event
        /// </summary>
        public void Resume() => resumeMusicEvent.Post(gameObject);

        /// <summary>
        /// Switch music event states
        /// </summary>
        public void SetState(AK.Wwise.State state)
        {
            // Set the state value
            state.SetValue();
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

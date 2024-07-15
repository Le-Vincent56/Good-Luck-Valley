using GoodLuckValley.Patterns.Singletons;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Audio.Music
{
    public class MusicManager : PersistentSingleton<MusicManager>
    {
        [SerializeField] private AK.Wwise.Event startMusicEvent;
        [SerializeField] private AK.Wwise.Event stopMusicEvent;
        [SerializeField] private AK.Wwise.Event pauseMusicEvent;
        [SerializeField] private AK.Wwise.Event resumeMusicEvent;
        [SerializeField] private List<uint> changedStates;

        protected override void Awake()
        {
            base.Awake();

            Play();
        }

        /// <summary>
        /// Play the music event
        /// </summary>
        public void Play() => startMusicEvent.Post(gameObject);

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
        /// <param name="state"></param>
        public void SetState(AK.Wwise.State state, bool permanentChange = false)
        {
            // If the state to change is already in the permanent list, return
            if (changedStates.Contains(state.Id))
            {
                return;
            }

            // Set the state value
            state.SetValue();

            // If noted as permanent, change the states
            if(permanentChange)
            {
                changedStates.Add(state.Id);
            }
        }
    }
}
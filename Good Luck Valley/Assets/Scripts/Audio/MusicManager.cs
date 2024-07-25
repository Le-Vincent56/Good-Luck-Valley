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
        [SerializeField] private List<uint> disabledStates;

        protected override void Awake()
        {
            base.Awake();   
        }

        private void Start()
        {
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
            // If the state is in the disabled list, return
            if (disabledStates.Contains(state.Id)) return;

            // Set the state value
            state.SetValue();

            // If noted as permanent, disable the state
            if(permanentChange)
                DisableState(state);
        }

        /// <summary>
        /// Disable a music event state
        /// </summary>
        /// <param name="state"></param>
        public void DisableState(AK.Wwise.State state)
        {
            // If the state is already disabled, return
            if (disabledStates.Contains(state.Id)) return;

            // Add the state to the disabled states
            disabledStates.Add(state.Id);
        }
    }
}
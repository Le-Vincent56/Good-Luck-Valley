using GoodLuckValley.Architecture.Singletons;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley
{
    public class MusicManager : PersistentSingleton<MusicManager>
    {
        [Header("Wwise Events")]
        [SerializeField] private AK.Wwise.Event startMusicEvent;
        [SerializeField] private AK.Wwise.Event stopMusicEvent;
        [SerializeField] private AK.Wwise.Event pauseMusicEvent;
        [SerializeField] private AK.Wwise.Event resumeMusicEvent;

        [Header("Fields")]
        [SerializeField] private List<uint> disabledStates;
        [SerializeField] private bool isPlaying;

        [SerializeField] private List<AK.Wwise.State> menuStates = new List<AK.Wwise.State>();
        [SerializeField] private List<AK.Wwise.State> gameStates = new List<AK.Wwise.State>();

        protected override void Awake()
        {
            base.Awake();
        }

        private void Start()
        {
            //SetMenuStates();

            //Play();
        }

        /// <summary>
        /// Play the music event
        /// </summary>
        public void Play()
        {
            if (isPlaying) return;

            startMusicEvent.Post(gameObject);

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
        /// Set the default menu states
        /// </summary>
        public void SetMenuStates(bool fromPause = false)
        {
            foreach (AK.Wwise.State state in menuStates)
            {
                SetState(state);
            }

            if (fromPause)
                disabledStates.Clear();
        }


        /// <summary>
        /// Set game states
        /// </summary>
        public void SetGameStates()
        {
            // Clear the disabled states
            disabledStates.Clear();

            foreach (AK.Wwise.State state in gameStates)
            {
                SetState(state);
            }
        }

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
            if (permanentChange)
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

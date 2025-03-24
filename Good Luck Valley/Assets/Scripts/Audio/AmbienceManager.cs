using GoodLuckValley.Architecture.Singletons;
using UnityEngine;

namespace GoodLuckValley.Audio.Ambience
{
    public class AmbienceManager : PersistentSingleton<AmbienceManager>
    {
        [Header("Wwise")]
        [SerializeField] private AK.Wwise.Event ambientBed;
        [SerializeField] private AK.Wwise.Event ambient2D;

        [SerializeField] private AK.Wwise.Event stopAmbientBed;
        [SerializeField] private AK.Wwise.Event stopAmbient2D;

        [SerializeField] private AK.Wwise.RTPC lotusDistanceRTPC;

        [SerializeField] private bool isPlayingAmbience;

        protected override void Awake()
        {
            // Call the parent Awake()
            base.Awake();

            // Set default RTPC values
            SetDefaultRTPCValues();
        }

        /// <summary>
        /// Set the default RTPC values
        /// </summary>
        private void SetDefaultRTPCValues()
        {
            // Set the farthest distance for the lotus
            lotusDistanceRTPC.SetGlobalValue(100f);
        }

        /// <summary>
        /// Start ambient sounds
        /// </summary>
        public void StartAmbience()
        {
            // Exit case - already playing ambience
            if (isPlayingAmbience) return;

            // Set default RTPC values
            SetDefaultRTPCValues();

            // Post the ambient events
            ambientBed.Post(gameObject);
            ambient2D.Post(gameObject);

            // State playing ambience
            isPlayingAmbience = true;
        }

        /// <summary>
        /// Stop ambient sounds
        /// </summary>
        public void StopAmbience()
        {
            // Exit case - already not playing ambience
            if (!isPlayingAmbience) return;

            // Post the stop ambient events
            stopAmbientBed.Post(gameObject);
            stopAmbient2D.Post(gameObject);

            // State not playing ambience
            isPlayingAmbience = false;
        }
    }
}

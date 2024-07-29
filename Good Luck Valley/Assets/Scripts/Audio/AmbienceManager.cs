using UnityEngine;
using GoodLuckValley.Patterns.Singletons;

namespace GoodLuckValley.Audio.Ambience
{
    public class AmbienceManager : PersistentSingleton<AmbienceManager>
    {
        [Header("Wwise")]
        [SerializeField] private AK.Wwise.Event ambientBed;
        [SerializeField] private AK.Wwise.Event ambient2D;

        [SerializeField] private AK.Wwise.Event stopAmbientBed;
        [SerializeField] private AK.Wwise.Event stopAmbient2D;

        [SerializeField] private bool isPlayingAmbience;

        /// <summary>
        /// Start ambient sounds
        /// </summary>
        public void StartAmbience()
        {
            // Exit case - already playing ambience
            if (isPlayingAmbience) return;

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
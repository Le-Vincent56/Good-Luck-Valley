using GoodLuckValley.Audio.Sound;
using GoodLuckValley.Extensions;
using GoodLuckValley.Patterns.Singletons;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace GoodLuckValley.Audio.Music
{
    public class MusicManager : PersistentSingleton<MusicManager>
    {
        private const float crossFadeTime = 1.0f;
        private float fading;
        public AudioSource current;
        private readonly Queue<AudioClip> playlist = new Queue<AudioClip>();
        private readonly Queue<bool> transitions = new Queue<bool>();
        private int crossFadeCount = 0;
        public bool canEnqueue;

        [SerializeField] private List<AudioClip> initialPlaylist;
        [SerializeField] private AudioMixerGroup musicMixerGroup;

        private void Start()
        {
            canEnqueue = true;

            // Loop through each clip added in the initial playlist
            foreach(AudioClip clip in initialPlaylist)
            {
                // Add the clips to the playlist
                AddToPlaylist(clip);
            }
        }

        private void Update()
        {
            // Handle any cross fading, but only one time
            if(crossFadeCount <= 0)
                HandleCrossFade();

            // Check if the next track should be played
            if (current && !current.isPlaying && playlist.Count > 0)
            {
                // Play the next track
                PlayNextTrack();
            }
        }

        /// <summary>
        /// Add an AudioClip to the playlist
        /// </summary>
        /// <param name="clip">The AudioClip to add to the playlist</param>
        public void AddToPlaylist(AudioClip clip, bool isTransition = false)
        {
            if (!canEnqueue) return;

            // Enqueue the AudioClip
            playlist.Enqueue(clip);
            transitions.Enqueue(!isTransition);

            Debug.Log("Enqueued " + clip.name);

            // Check if any audio is playing
            if(current == null)
            {
                // If not, play the next track
                PlayNextTrack();
            }
        }

        /// <summary>
        /// Play the next track in the playlist
        /// </summary>
        public void PlayNextTrack()
        {
            // Check if there's any AudioClip to dequeue
            if(playlist.TryDequeue(out AudioClip nextTrack))
            {
                // If so, play the dequeue'd AudioClip
                Play(nextTrack);
            }
        }

        /// <summary>
        /// Play an AudioClip
        /// </summary>
        /// <param name="clip">The AudioClip to play</param>
        public void Play(AudioClip clip)
        {
            // Return if this AudioClip is already playing
            if (current && current.clip == clip) return;

            // Set the current AudioSource
            current = gameObject.GetOrAdd<AudioSource>();
            current.clip = clip;
            current.outputAudioMixerGroup = musicMixerGroup;
            current.loop = transitions.Dequeue();
            current.volume = (crossFadeCount <= 0) ? 0f : 1f;
            current.bypassListenerEffects = true;

            // Play the current AudioSource
            current.Play();

            // Set the fading
            fading = 0.001f;
        }

        /// <summary>
        /// Handle the cross fading between the previous and current AudioSources
        /// </summary>
        private void HandleCrossFade()
        {
            // Return if the fading is done
            if (fading <= 0f) return;

            fading += Time.deltaTime;

            float fraction = Mathf.Clamp01(fading / crossFadeTime);

            // Logarithmic fade
            float logFraction = fraction.ToLogarithmicFraction();

            // If a current AudioSource is present, increase the volume
            if (current) current.volume = logFraction;

            // Check if the fraction is equal to/exceeded one
            if(fraction >= 1)
            {
                // Set fading to 0
                fading = 0.0f;

                crossFadeCount++;
            }
        }
    }
}
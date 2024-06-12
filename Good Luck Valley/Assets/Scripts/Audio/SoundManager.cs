using GoodLuckValley.Patterns.Singletons;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Pool;

namespace GoodLuckValley.Audio.Sound
{
    public class SoundManager : PersistentSingleton<SoundManager>
    {
        private IObjectPool<SoundEmitter> soundEmitterPool;
        private readonly List<SoundEmitter> activeSoundEmitters = new List<SoundEmitter>();
        public readonly LinkedList<SoundEmitter> FrequentSoundEmitters = new LinkedList<SoundEmitter>();

        [SerializeField] private SoundEmitter soundEmitterPrefab;
        [SerializeField] private bool collectionCheck = true;
        [SerializeField] private int defaultCapacity = 10;
        [SerializeField] private int maxPoolSize = 100;
        [SerializeField] private int maxSoundInstances = 30;

        private void Start()
        {
            InitializePool();
        }

        /// <summary>
        /// Initialize the SoundEmitter pool
        /// </summary>
        private void InitializePool()
        {
            soundEmitterPool = new ObjectPool<SoundEmitter>(
                CreateSoundEmitter,
                OnTakeFromPool,
                OnReturnedToPool,
                OnDestroyPoolObject,
                collectionCheck,
                defaultCapacity,
                maxPoolSize
            );
        }

        /// <summary>
        /// Create a sound
        /// </summary>
        /// <returns>The SoundBuilder used to create the sound</returns>
        public SoundBuilder CreateSoundBuilder() => new SoundBuilder(this);

        /// <summary>
        /// Check if a sound can be played
        /// </summary>
        /// <param name="data">The sound to check</param>
        /// <returns>True if the sound can be played, false if the data is not found
        /// in the SoundData dictionary or if the sound cannot be played</returns>
        public bool CanPlaySound(SoundData data)
        {
            // If the sound is not marked as a frequent sound, assume
            // that we can always play it
            if (!data.frequentSound) return true;

            // Check if the amount of frequent soudn emitters is equal to
            // or exceeds the number of max sound instances
            if(FrequentSoundEmitters.Count >= maxSoundInstances)
            {
                // Attempt to stop the sound
                try
                {
                    // Stop the sound emitter
                    FrequentSoundEmitters.First.Value.Stop();
                    return true;
                }
                catch
                {
                    Debug.Log("SoundEmitter is already released");
                }

                return false;
            }

            return true;
        }

        /// <summary>
        /// Get a SoundEmitter from the pool
        /// </summary>
        /// <returns></returns>
        public SoundEmitter Get()
        {
            // Get the SoundEmitter from the pool 
            return soundEmitterPool.Get();
        }

        /// <summary>
        /// Return a SoundEmitter to the pool
        /// </summary>
        /// <param name="soundEmitter"></param>
        public void ReturnToPool(SoundEmitter soundEmitter)
        {
            // Release the sound emitter back to the pool
            soundEmitterPool.Release(soundEmitter);
        }

        /// <summary>
        /// Callback function to create a SoundEmitter for the pool
        /// </summary>
        /// <returns>The SoundEmitter being created</returns>
        private SoundEmitter CreateSoundEmitter()
        {
            // Instantiate the prefab and set as inactive
            SoundEmitter soundEmitter = Instantiate(soundEmitterPrefab);
            soundEmitter.gameObject.SetActive(false);
            return soundEmitter;
        }

        /// <summary>
        /// Callback function for a SoundEmitter pool object being taken from the pool
        /// </summary>
        /// <param name="soundEmitter">The SoundEmitter being taken from the pool</param>
        private void OnTakeFromPool(SoundEmitter soundEmitter)
        {
            // Set the game object to active
            soundEmitter.gameObject.SetActive(true);

            // Add it to the active list
            activeSoundEmitters.Add(soundEmitter);
        }

        /// <summary>
        /// Callback function for the return of a SoundEmitter pool object to the pool
        /// </summary>
        /// <param name="soundEmitter">The SoundEmitter being returned to the pool</param>
        private void OnReturnedToPool(SoundEmitter soundEmitter)
        {
            // Check if the SoundEmitter Node is null
            if(soundEmitter.Node != null)
            {
                // If so, remove the node from the linked list
                FrequentSoundEmitters.Remove(soundEmitter.Node);

                // Nullify the SoundEmitter node
                soundEmitter.Node = null;
            }

            // De-activate the game object
            soundEmitter.gameObject.SetActive(false);

            // Remove it from the active list
            activeSoundEmitters.Remove(soundEmitter);
        }

        /// <summary>
        /// Callback function for the destruction of a SoundEmitter pool object
        /// </summary>
        /// <param name="soundEmitter">The SoundEmitter being destroyed</param>
        private void OnDestroyPoolObject(SoundEmitter soundEmitter)
        {
            // destroy the game object
            Destroy(soundEmitter.gameObject);
        }

        /// <summary>
        /// Stop all sounds
        /// </summary>
        public void StopAll()
        {
            // Loop through each active sound emitter
            foreach (SoundEmitter soundEmitter in activeSoundEmitters)
            {
                // Stop the sound emitter
                soundEmitter.Stop();
            }

            // Clear the frequent soudn emitters
            FrequentSoundEmitters.Clear();
        }
    }
}

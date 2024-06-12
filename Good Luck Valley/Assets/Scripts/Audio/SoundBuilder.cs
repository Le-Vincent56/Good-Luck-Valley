using UnityEngine;

namespace GoodLuckValley.Audio.Sound
{
    public class SoundBuilder
    {
        private readonly SoundManager soundManager;
        private SoundData soundData;
        private Vector2 position = Vector2.zero;
        private bool randomPitch;

        public SoundBuilder(SoundManager soundManager)
        {
            this.soundManager = soundManager;
        }

        public SoundBuilder WithSoundData(SoundData soundData)
        {
            this.soundData = soundData;
            return this;
        }

        public SoundBuilder WithPosition(Vector2 position)
        {
            this.position = position;
            return this;
        }

        public SoundBuilder WithRandomPitch()
        {
            this.randomPitch = true;
            return this;
        }

        public void Play()
        {
            // Check if a SoundData is present
            if (soundData == null)
            {
                // If not, debug and return
                Debug.LogError("SoundData is null");
                return;
            }

            // Return if the sound cannot be played
            if (!soundManager.CanPlaySound(soundData)) return;

            // Get a SoundEitter
            SoundEmitter soundEmitter = soundManager.Get();

            // Set the SoundEmitter's transform position and parent
            soundEmitter.transform.position = position;
            soundEmitter.transform.parent = soundManager.transform;
            
            // Check if the pitch should be randomized
            if(randomPitch)
                // Randomize the pitch
                soundEmitter.WithRandomPitch();

            // Check if the sound is a frequent sound
            if (soundData.frequentSound)
                // Add the sound emitter as the last value in the linked list and set the node
                soundEmitter.Node = soundManager.FrequentSoundEmitters.AddLast(soundEmitter);

            // Play the sound emitter
            soundEmitter.Play();
        }
    }
}

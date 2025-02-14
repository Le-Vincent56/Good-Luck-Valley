using UnityEngine;

namespace GoodLuckValley.World.Episodes
{
    [ExecuteInEditMode]
    public class ParticlePlayer : MonoBehaviour
    {
        private ParticleSystem[] particles;

        private void OnValidate()
        {
            // Get the particle systems
            particles = GetComponentsInChildren<ParticleSystem>();
        }

        /// <summary>
        /// Play all the particles
        /// </summary>
        public void PlayParticles()
        {
            // Iterate through all of the particle systems
            foreach (ParticleSystem particle in particles)
            {
                // Play the particle system
                particle.Play();
            }
        }
    }
}

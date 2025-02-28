using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.World.Episodes
{
    [ExecuteInEditMode]
    public class ParticlePlayer : MonoBehaviour
    {
        private List<ParticleSystem> particles;

        private void OnValidate()
        {
            particles = new List<ParticleSystem>();

            // Get the particle systems to be seen in Timeline updates
            GetComponentsInChildren(particles);
        }

        private void Start()
        {
            particles = new List<ParticleSystem>();

            // Get the particle systems
            GetComponentsInChildren(particles);
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

        /// <summary>
        /// Stop all the particles
        /// </summary>
        public void StopParticles()
        {
            // Iterate through all of the particle systems
            foreach (ParticleSystem particle in particles)
            {
                // Play the particle system
                particle.Stop();
            }
        }
    }
}

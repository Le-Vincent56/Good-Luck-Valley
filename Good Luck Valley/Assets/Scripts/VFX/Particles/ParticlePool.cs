using UnityEngine;
using UnityEngine.Pool;

namespace GoodLuckValley.VFX.Particles
{
    public class ParticlePool
    {
        private readonly ParticleSystem particlesPrefab;
        private readonly ObjectPool<ParticleSystem> pool;

        /// <summary>
        /// Create the Particle Pool
        /// </summary>
        public ParticlePool(ParticleSystem particlesPrefab)
        {
            // Set the Particles Prefab
            this.particlesPrefab = particlesPrefab;

            // Create the Particle Pool
            pool = new ObjectPool<ParticleSystem>(
                CreateParticles,
                OnTakeParticlesFromPool,
                OnReturnParticlesToPool,
                OnDestroyParticles,
                true,
                10,
                100
            );
        }

        /// <summary>
        /// Get a Particle System from the Particle Pool
        /// </summary>
        public ParticleSystem Get() => pool.Get();

        /// <summary>
        /// Release a Particle System back to the Particle Pool
        /// </summary>
        public void Release(ParticleSystem particles) => pool.Release(particles);

        /// <summary>
        /// Instantiate a Particle System within the Particle Pool
        /// </summary>
        private ParticleSystem CreateParticles()
        {
            // Instantiate the Particle System
            ParticleSystem particles = Object.Instantiate(particlesPrefab);

            return particles;
        }

        /// <summary>
        /// Take a Particle System from the Particle Pool
        /// </summary>
        private void OnTakeParticlesFromPool(ParticleSystem particles)
        {
            // Stop the particles if they're playing
            if (particles.isPlaying) particles.Stop();

            // Set the Particle System to be active
            particles.gameObject.SetActive(true);
        }

        /// <summary>
        /// Return a Particle System to the Particle Pool
        /// </summary>
        private void OnReturnParticlesToPool(ParticleSystem particles)
        {
            // Stop the particles
            particles.Stop();

            // Deactivate the Particle System
            particles.gameObject.SetActive(false);
        }

        /// <summary>
        /// Destroy a Particle System within the Particle Pool
        /// </summary>
        private void OnDestroyParticles(ParticleSystem particles)
        {
            // Destroy the Particle System
            Object.Destroy(particles.gameObject);
        }
    }
}

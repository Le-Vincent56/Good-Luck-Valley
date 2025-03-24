using DG.Tweening;
using GoodLuckValley.Timers;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley
{
    public class TimeWarpParticleController : MonoBehaviour
    {
        private List<ParticleSystem> particles;
        private ParticleSystem ambientParticles;
        private ParticleSystem burstParticles;


        private void Awake()
        {
            // Get particles
            particles = new List<ParticleSystem>();
            GetComponentsInChildren(particles);
            ambientParticles = particles[0];
            burstParticles = particles[1];

            // Play the ambient particles
            ambientParticles.Play();
        }

        /// <summary>
        /// Play the burst particles
        /// </summary>
        public void Burst() => burstParticles.Play();
    }
}

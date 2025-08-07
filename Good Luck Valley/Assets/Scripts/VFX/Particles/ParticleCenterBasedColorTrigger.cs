using System.Collections.Generic;
using GoodLuckValley.World.Triggers;
using UnityEngine;

namespace GoodLuckValley.VFX.Particles
{
    public class ParticleCenterBasedColorTrigger : CenterDistanceBasedTrigger
    {
        [Header("Particle System Settings")]
        [SerializeField] private List<ParticleSystem> particleSystems;
        private List<ParticleSystem.MainModule> mainModules;
        private List<ParticleSystem.Particle[]> particles;
        
        [Header("Effect Settings")]
        [SerializeField] private Gradient colorGradient;
        [SerializeField] private bool updateExistingParticles = true;
        [SerializeField] private bool blendByParticleAge = false;
        
        protected override void Awake()
        {
            // Get box collider elements
            base.Awake();

            // Exit case - if the particle system is null or doesn't have elements
            if (particleSystems is not { Count: > 0 }) return;
            
            // Initialize the lists
            mainModules = new List<ParticleSystem.MainModule>(particleSystems.Count);
            particles = new List<ParticleSystem.Particle[]>(particleSystems.Count);

            // Iterate through each particle system
            foreach (ParticleSystem ps in particleSystems)
            {
                // Extract the Particle System's Main Module
                ParticleSystem.MainModule module = ps.main;
 
                // Store the main module
                mainModules.Add(module);
                
                // Add an array to store particles
                particles.Add(new ParticleSystem.Particle[ps.main.maxParticles]);
            }
        }

        private void OnDestroy()
        {
            // Iterate through each particle array
            for (int i = 0; i < particles.Count; i++)
            {
                // Clean up the particle array
                particles[i] = null;
            }
        }

        protected override void ApplyEffects(float intensity)
        {
            // Exit case - none of the lists are initialized or have data
            if (particleSystems is not { Count: > 0 }) return;
            if (mainModules is not { Count: > 0 }) return;

            // Evaluate the color in the gradient at the appropriate, calculated intensity
            Color newColor = colorGradient.Evaluate(intensity);

            // Iterate through each main module
            for (int i = 0; i < mainModules.Count; i++)
            {
                // Set the new color for starting colors
                ParticleSystem.MainModule mainModule = mainModules[i];
                mainModule.startColor = newColor;
                mainModules[i] = mainModule;

                // Skip if not updating existing particles
                if (!updateExistingParticles) continue;
                
                // Update existing colors
                UpdateExistingParticleColors(i, newColor);
            }
        }

        private void UpdateExistingParticleColors(int index, Color newColor)
        {
            ParticleSystem ps = particleSystems[index];
            ParticleSystem.Particle[] particlesArray = particles[index];
            
            // Get the number of particles emitted from the system currently
            int particleCount = ps.particleCount;
            
            // Check if the corresponding particles array is not created or doesn't
            // have enough space
            if (particlesArray == null || particlesArray.Length < particleCount)
            {
                // Resize the array if needed
                particlesArray = new ParticleSystem.Particle[particleCount * 2];
            }
            
            // Get all particles
            int actualParticleCount = ps.GetParticles(particlesArray);
            
            // Update each particle's color
            for (int i = 0; i < actualParticleCount; i++)
            {
                ParticleSystem.Particle particle = particlesArray[i];
                
                if (blendByParticleAge)
                {
                    // Blend color based on particle age (newer particles adapt faster)
                    float particleLifePercent = particle.remainingLifetime / particle.startLifetime;
                    Color currentColor = particle.GetCurrentColor(ps);
                    particle.startColor = Color.Lerp(currentColor, newColor, 1f - particleLifePercent);
                }
                else
                {
                    // Immediate color change
                    particle.startColor = newColor;
                }
            }
            
            // Apply the changes back to the particle system
            ps.SetParticles(particlesArray, actualParticleCount);
        }
    }
}

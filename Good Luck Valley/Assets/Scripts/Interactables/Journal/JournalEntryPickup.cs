using GoodLuckValley.UI.Journal.Model;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace GoodLuckValley.Interactables.Journal
{
    public class JournalEntryPickup : Collectible
    {
        [SerializeField] private JournalData journalData;
        private ParticleSystem particles;
        private Light2D noteLight;

        protected override void Awake()
        {
            // Call the parent Awake()
            base.Awake();

            // Set the strategy
            strategy = new EntryPickupStrategy(journalData);

            // Get components
            particles = GetComponentInChildren<ParticleSystem>();
            noteLight = GetComponentInChildren<Light2D>();
        }

        protected override void Collect()
        {
            // Stop the Particle System
            particles.Stop();
            noteLight.gameObject.SetActive(false);
        }
    }
}

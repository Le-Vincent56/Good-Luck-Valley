using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.UI.Cinematics
{
    public class CinematicBarHandler : MonoBehaviour
    {
        [SerializeField] private float duration;
        private CinematicBar[] cinematicBars;

        private void Awake()
        {
            // Get components
            cinematicBars = GetComponentsInChildren<CinematicBar>();
        }

        /// <summary>
        /// Handle showing and hiding the Cinematic Bars
        /// </summary>
        public void HandleCinematicBars(Component sender, object data)
        {
            // Verify the correct data was sent
            if (data is not bool) return;

            // Cast data
            bool startCinematic = (bool)data;

            // Check if starting a cinematci
            if(startCinematic)
            {
                // Iterate through each Cinematic Bar
                foreach(CinematicBar bar in cinematicBars)
                {
                    // Show the bar
                    bar.Show(duration);
                }
            } 
            else
            {
                // Iterate through each Cinematic Bar
                foreach (CinematicBar bar in cinematicBars)
                {
                    // Hide the bar
                    bar.Hide(duration);
                }
            }
        }
    }
}
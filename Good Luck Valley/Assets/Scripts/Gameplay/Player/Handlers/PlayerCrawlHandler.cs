using UnityEngine;
using GoodLuckValley.Gameplay.Player.Data;
using GoodLuckValley.Gameplay.Player.Interfaces;

namespace GoodLuckValley.Gameplay.Player.Handlers
{
    /// <summary>
    /// Handles the crawling functionality for a player, including starting, ending,
    /// and updating crawling state, as well as determining the player's ability to stand.
    /// </summary>
    public class PlayerCrawlHandler : ICrawlHandler
    {
        private readonly PlayerStats _stats;

        private bool _isCrawling;
        private float _crawlStartTime;
        private float _elapsed;
        private bool _canStand = true;

        public bool IsCrawling => _isCrawling;
        public bool CanStand => _canStand;

        public float SpeedModifier
        {
            get
            {
                if (!_isCrawling) return 1f;

                float t = Mathf.InverseLerp(0f, _stats.CrouchSlowDownTime, _elapsed);
                return Mathf.Lerp(1f, _stats.CrouchSpeedModifier, t);
            }
        }

        public PlayerCrawlHandler(PlayerStats stats) => _stats = stats;

        /// <summary>
        /// Initiates the player's crawling state, setting the relevant starting state and timing values.
        /// </summary>
        /// <param name="currentTime">The current game time at which crawling starts, used for state initialization.</param>
        public void StartCrawl(float currentTime)
        {
            _isCrawling = true;
            _crawlStartTime = currentTime;
            _elapsed = 0f;
        }

        /// <summary>
        /// Ends the player's crawling state, clearing any associated state or timers.
        /// </summary>
        public void EndCrawl()
        {
            _isCrawling = false;
            _elapsed = 0f;
        }

        /// <summary>
        /// Updates the player's ability to stand based on the state of the ceiling.
        /// </summary>
        /// <param name="ceilingBlocked">
        /// Indicates whether the ceiling is obstructed.
        /// If true, the player cannot stand; if false, the player can stand.
        /// </param>
        public void UpdateCeilingState(bool ceilingBlocked) => _canStand = !ceilingBlocked;

        /// <summary>
        /// Updates the crawling state and timing logic, applying time-dependent changes to the crawl behavior.
        /// </summary>
        /// <param name="deltaTime">The time elapsed since the last update, used for calculating timing-dependent updates.</param>
        /// <param name="currentTime">The current game time, used to track the crawl's duration and enforce timing behavior.</param>
        public void Tick(float deltaTime, float currentTime)
        {
            if (!_isCrawling) return;

            _elapsed += deltaTime;
        }
    }
}
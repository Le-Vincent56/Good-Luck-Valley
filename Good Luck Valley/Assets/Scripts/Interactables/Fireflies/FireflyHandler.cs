using GoodLuckValley.Architecture.Optionals;
using GoodLuckValley.Audio;
using UnityEngine;

namespace GoodLuckValley.Interactables.Fireflies
{
    public class FireflyHandler : MonoBehaviour
    {
        private PlayerSFX playerSFX;

        private Optional<Fruit> fruit = Optional<Fruit>.None();
        private Optional<Fireflies> fireflies = Optional<Fireflies>.None();

        private void Awake()
        {
            // Get components
            playerSFX = GetComponentInChildren<PlayerSFX>();
        }

        /// <summary>
        /// Set a Fruit to be handled
        /// </summary>
        public void SetFruit(Optional<Fruit> fruit) => this.fruit = fruit;

        /// <summary>
        /// Get the Fruit being handled, if it exists
        /// </summary>
        public Optional<Fruit> GetFruit() => fruit;

        /// <summary>
        /// Set Fireflies to be handled
        /// </summary>
        public void SetFireflies(Optional<Fireflies> fireflies)
        {
            this.fireflies = fireflies;

            // Check if the player has fireflies
            fireflies.Match(
                onValue: fireflies =>
                {
                    // Add fireflies
                    playerSFX.AddFireflies();
                    return 0;
                },
                onNoValue: () =>
                {
                    // Remove fireflies
                    playerSFX.RemoveFireflies();
                    return 0;
                }
            );
        }

        /// <summary>
        /// Get the Fireflies being handled, if it exists
        /// </summary>
        public Optional<Fireflies> GetFireflies() => fireflies;
    }
}

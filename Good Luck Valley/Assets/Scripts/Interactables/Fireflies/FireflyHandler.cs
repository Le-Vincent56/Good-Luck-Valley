using GoodLuckValley.Architecture.Optionals;
using UnityEngine;

namespace GoodLuckValley.Interactables.Fireflies
{
    public class FireflyHandler : MonoBehaviour
    {
        private Optional<Fruit> fruit = Optional<Fruit>.None();
        private Optional<Fireflies> fireflies = Optional<Fireflies>.None();

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
        public void SetFireflies(Optional<Fireflies> fireflies) => this.fireflies = fireflies;

        /// <summary>
        /// Get the Fireflies being handled, if it exists
        /// </summary>
        public Optional<Fireflies> GetFireflies() => fireflies;
    }
}

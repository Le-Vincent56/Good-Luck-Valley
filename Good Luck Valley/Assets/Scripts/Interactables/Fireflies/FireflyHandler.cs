using GoodLuckValley.Architecture.Optionals;
using UnityEngine;

namespace GoodLuckValley.Interactables.Fireflies
{
    public class FireflyHandler : MonoBehaviour
    {
        private Optional<Fruit> fruit = Optional<Fruit>.None();

        /// <summary>
        /// Set a Fruit to be handled
        /// </summary>
        public void SetFruit(Optional<Fruit> fruit) => this.fruit = fruit;

        /// <summary>
        /// Get the Fruit being handled, if it exists
        /// </summary>
        public Optional<Fruit> GetFruit() => fruit;
    }
}

using GoodLuckValley.Architecture.Optionals;
using UnityEngine;

namespace GoodLuckValley.Interactables.Fireflies
{
    public class Fruit : Collectible
    {
        protected override void Awake()
        {
            // Call the parent Awake()
            base.Awake();

            // Set the strategy
            strategy = new FruitStrategy(this);
        }

        /// <summary>
        /// Use the Fruit
        /// </summary>
        public void Use(InteractableHandler handler)
        {
            // Remove the Firefly Handler's Fruit
            handler.FireflyHandler.SetFruit(Optional<Fruit>.NoValue);

            // Fade in the Fruit and allow interaction
            FadeInteractable(1f, fadeDuration, () => canInteract = true);
        }
    }
}

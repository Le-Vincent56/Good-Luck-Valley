using UnityEngine;

namespace GoodLuckValley.Interactables.Fireflies
{
    public class FireflyStrategy : InteractableStrategy
    {
        private Fireflies parent;

        public FireflyStrategy(Fireflies parent)
        {
            this.parent = parent;
        }

        public override bool Interact(InteractableHandler handler)
        {
            // Set the default failed state
            bool success = false;

            // Check if the Firefly Handler has a fruit
            handler.FireflyHandler.GetFruit().Match(
                onValue: fruit =>
                {
                    // If there's a Fruit, use it
                    fruit.Use(handler);

                    // Get the Fireflies to follow the Player
                    parent.Follow(handler.transform);

                    // Set successful
                    success = true;

                    return 0;
                },
                onNoValue: () =>
                {
                    // The Firefly Handler does not have a Fruit, so do nothing
                    return 0;
                }
            );

            Debug.Log($"Interacted with FIreflies: {success}");

            // Return the success state
            return success;
        }
    }
}

using GoodLuckValley.Architecture.Optionals;
using UnityEngine;

namespace GoodLuckValley.Interactables.Fireflies
{
    public class LanternStrategy : InteractableStrategy
    {
        private readonly Lantern parent;
        public LanternStrategy(Lantern parent)
        {
            // Set the parent Interactable
            this.parent = parent;
        }

        public override bool Interact(InteractableHandler handler)
        {
            // Set the default failed state
            bool success = false;

            // Check if the Firefly Handler has a fruit
            handler.FireflyHandler.GetFireflies().Match(
                onValue: fireflies =>
                {
                    // If there's a Fruit, use it
                    fireflies.Follow(parent.transform);

                    // Remove the Fireflies from the Firefly Handler
                    handler.FireflyHandler.SetFireflies(Optional<Fireflies>.NoValue);

                    // Activate the lantern
                    parent.Activate();

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

            // Return the success state
            return success;
        }
    }
}

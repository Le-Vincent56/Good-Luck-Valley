namespace GoodLuckValley.Interactables.Fireflies
{
    public class FireflyStrategy : InteractableStrategy
    {
        private readonly Fireflies parent;

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
                    parent.Follow(handler.Controller.FollowTransform);

                    // Set the fireflies to the Firefly Handler
                    handler.FireflyHandler.SetFireflies(parent);

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

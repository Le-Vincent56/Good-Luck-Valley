using UnityEngine;

namespace GoodLuckValley.Interactables.Fireflies
{
    public class FruitStrategy : InteractableStrategy
    {
        private Fruit parent;

        public FruitStrategy(Fruit parent)
        {
            // Set the parent Interactable
            this.parent = parent;
        }

        public override bool Interact(InteractableHandler handler)
        {
            // Exit case - the Firefly Handler already has a Fruit
            if (handler.FireflyHandler.GetFruit().HasValue) return false;

            // Set the FireflyHandler to have a Fruit
            handler.FireflyHandler.SetFruit(parent);

            Debug.Log("Fruit collected");

            return true;
        }
    }
}

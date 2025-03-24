using UnityEngine;

namespace GoodLuckValley.Interactables.Mushroom
{
    public class MushroomPickup : Collectible
    {
        [SerializeField] private int hash;

        public int Hash => hash;

        protected override void Awake()
        {
            // Call the parent Awake()
            base.Awake();

            // Set the strategy
            strategy = new MushroomPickupStrategy(this);
        }
    }
}

using UnityEngine;

namespace GoodLuckValley.Interactables.Journal
{
    public class JournalPickup : Interactable
    {
        [SerializeField] private int hash;

        public int Hash => hash;

        protected override void Awake()
        {
            // Call the parent Awake()
            base.Awake();

            // Set the strategy
            strategy = new JournalPickupStrategy(this);
        }
    }
}

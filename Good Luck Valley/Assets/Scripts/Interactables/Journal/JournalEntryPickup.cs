using GoodLuckValley.UI.Journal.Model;
using UnityEngine;

namespace GoodLuckValley.Interactables.Journal
{
    public class JournalEntryPickup : Collectible
    {
        [SerializeField] private JournalData journalData;

        protected override void Awake()
        {
            // Call the parent Awake()
            base.Awake();

            // Set the strategy
            strategy = new EntryPickupStrategy(journalData);
        }
    }
}

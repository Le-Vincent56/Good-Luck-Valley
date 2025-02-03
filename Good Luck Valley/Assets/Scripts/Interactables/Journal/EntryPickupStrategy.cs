using GoodLuckValley.UI.Journal.Model;
using GoodLuckValley.Events;
using GoodLuckValley.Events.Journal;

namespace GoodLuckValley.Interactables.Journal
{
    public class EntryPickupStrategy : InteractableStrategy
    {
        private readonly JournalData data;

        public EntryPickupStrategy(JournalData data)
        {
            this.data = data;
        }

        public override bool Interact(InteractableHandler handler)
        {
            // Add a Journal Entry
            EventBus<UnlockJournalEntry>.Raise(new UnlockJournalEntry()
            {
                Data = data
            });

            return true;
        }
    }
}

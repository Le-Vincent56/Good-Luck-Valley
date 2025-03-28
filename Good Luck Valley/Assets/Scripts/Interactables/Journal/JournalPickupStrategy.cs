using GoodLuckValley.Events;
using GoodLuckValley.Events.Cinematics;
using GoodLuckValley.Events.Journal;

namespace GoodLuckValley.Interactables.Journal
{
    public class JournalPickupStrategy : InteractableStrategy
    {
        public JournalPickupStrategy() { }

        public override bool Interact(InteractableHandler handler)
        {
            // Unlock the Journal
            EventBus<UnlockJournal>.Raise(new UnlockJournal());

            return true;
        }
    }
}

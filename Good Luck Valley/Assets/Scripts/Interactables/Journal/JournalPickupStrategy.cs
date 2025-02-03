namespace GoodLuckValley.Interactables.Journal
{
    public class JournalPickupStrategy : InteractableStrategy
    {
        private JournalPickup parent;

        public JournalPickupStrategy(JournalPickup parent)
        {
            // Set the parent Interactable
            this.parent = parent;
        }

        public override bool Interact(InteractableHandler handler)
        {
            // Unlock the Journal

            return true;
        }
    }
}

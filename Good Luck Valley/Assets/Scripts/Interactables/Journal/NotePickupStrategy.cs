namespace GoodLuckValley.Interactables.Journal
{
    public class NotePickupStrategy : InteractableStrategy
    {
        private Note parent;

        public NotePickupStrategy(Note parent)
        {
            this.parent = parent;
        }

        public override bool Interact(InteractableHandler handler)
        {
            // Pick up the note

            return true;
        }
    }
}

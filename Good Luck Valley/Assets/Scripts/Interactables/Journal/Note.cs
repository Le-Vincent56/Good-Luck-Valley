namespace GoodLuckValley.Interactables.Journal
{
    public class Note : Collectible
    {


        protected override void Awake()
        {
            // Call the parent Awake()
            base.Awake();

            // Set the strategy
            strategy = new NotePickupStrategy(this);
        }
    }
}

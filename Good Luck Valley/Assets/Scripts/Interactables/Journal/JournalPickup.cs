namespace GoodLuckValley.Interactables.Journal
{
    public class JournalPickup : Collectible
    {
        protected override void Awake()
        {
            // Call the parent Awake()
            base.Awake();

            // Set the strategy
            strategy = new JournalPickupStrategy();
        }
    }
}

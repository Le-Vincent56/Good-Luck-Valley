namespace GoodLuckValley.UI.Notifications
{
    public class NoteNotification : JournalNotification
    {
        public NoteNotification(int numCollected, NotificationPanel panel, int journalPageIndex) : base(panel, journalPageIndex)
        {
            headerText = "New Note!";
            descriptorText = $"Collected ({numCollected}/3)";
        }
    }
}
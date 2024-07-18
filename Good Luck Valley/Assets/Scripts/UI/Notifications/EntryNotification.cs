namespace GoodLuckValley.UI.Notifications
{
    public class EntryNotification : JournalNotification
    {
        public EntryNotification(string entryName, NotificationPanel panel, int journalPageIndex) : base(panel, journalPageIndex)
        {
            headerText = "Entry Unlocked";
            descriptorText = $"Can now read \"{entryName}\"";
        }
    }
}
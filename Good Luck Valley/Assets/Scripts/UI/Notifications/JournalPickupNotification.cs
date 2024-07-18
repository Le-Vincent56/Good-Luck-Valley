namespace GoodLuckValley.UI.Notifications
{
    public class JournalPickupNotification : JournalNotification
    {
        public JournalPickupNotification(NotificationPanel panel, int journalPageIndex) : base(panel, journalPageIndex) 
        {
            headerText = "Mysterious Journal";
            descriptorText = "There's something written in here!";
        }
    }
}
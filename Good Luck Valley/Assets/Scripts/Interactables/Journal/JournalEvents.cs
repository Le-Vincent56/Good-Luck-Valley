using GoodLuckValley.UI.Journal.Model;

namespace GoodLuckValley.Events.Journal
{
    public struct UnlockJournal : IEvent { }
    public struct UnlockJournalEntry : IEvent
    {
        public JournalData Data;
    }
}

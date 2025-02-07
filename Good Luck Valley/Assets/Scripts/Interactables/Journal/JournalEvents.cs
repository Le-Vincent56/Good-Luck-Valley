using GoodLuckValley.UI.Journal.Model;

namespace GoodLuckValley.Events.Journal
{
    public struct UnlockJournal : IEvent { }
    public struct UnlockJournalEntry : IEvent
    {
        public JournalData Data;
    }

    public struct ShowJournalPause : IEvent { }
    public struct HideJournalPause : IEvent { }

    public struct UpdateJournalUnlock : IEvent 
    {
        public bool Unlocked;
    }
}

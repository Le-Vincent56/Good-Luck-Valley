using UnityEngine;

namespace GoodLuckValley.UI.Journal.Model
{
    public class JournalEntry
    {
        [SerializeField] private JournalData journalData;

        public JournalData Data { get => journalData; }

        public JournalEntry(JournalData journalData)
        {
            this.journalData = journalData;
        }
    }
}

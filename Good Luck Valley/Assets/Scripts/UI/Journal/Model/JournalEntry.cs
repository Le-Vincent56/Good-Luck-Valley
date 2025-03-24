using UnityEngine;

namespace GoodLuckValley.UI.Journal.Model
{
    public class JournalEntry
    {
        [SerializeField] private JournalData journalData;
        private bool unlocked;

        public JournalData Data { get => journalData; }
        public bool Unlocked { get => unlocked; set => unlocked = value; }

        public JournalEntry(JournalData journalData)
        {
            this.journalData = journalData;
            unlocked = journalData.InitiallyUnlocked;
        }
    }
}

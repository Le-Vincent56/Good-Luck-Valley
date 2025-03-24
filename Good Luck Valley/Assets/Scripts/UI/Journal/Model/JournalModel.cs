using GoodLuckValley.Architecture.ObservableList;
using UnityEngine;

namespace GoodLuckValley.UI.Journal.Model
{
    public class JournalModel
    {
        private readonly ObservableList<JournalEntry> journalEntries;

        public ObservableList<JournalEntry> JournalEntries { get => journalEntries; }

        public JournalModel()
        {
            journalEntries = new ObservableList<JournalEntry>();
        }

        /// <summary>
        /// Add a Journal Entry to the Journal Model
        /// </summary>
        public void Add(JournalEntry journalEntry)
        {
            journalEntries.Add(journalEntry);
        }

        /// <summary>
        /// Unlock the Journal Entry associated with the Journal Data
        /// </summary>
        public void Unlock(JournalData data)
        {
            // Get the Journal Entry associated with the Journal Data
            JournalEntry entry = null;

            // Iterate through each Journal Entry
            foreach(JournalEntry journalEntry in journalEntries)
            {
                // Check if the Journal Entry's Data matches the specified Journal Data
                if (journalEntry.Data.Index == data.Index)
                {
                    // Set the Journal Entry to the found Journal Entry
                    entry = journalEntry;
                    break;
                }
            }

            // Exit case - the Journal Entry was not found
            if (entry == null) return;

            // Set the Journal Entry to unlocked
            entry.Unlocked = true;

            // Invoke the Observable List
            journalEntries.Invoke();
        }

        /// <summary>
        /// Unlock the Journal Entry at the specified index
        /// </summary>
        public void Unlock(int index)
        {
            // Get the Journal Entry at the specified index and unlock it
            journalEntries[index].Unlocked = true;

            // Invoke the Observable List
            journalEntries.Invoke();
        }
    }
}

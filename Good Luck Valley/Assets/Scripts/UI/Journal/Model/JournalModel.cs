using GoodLuckValley.Architecture.ObservableList;
using System.Linq;

namespace GoodLuckValley.UI.Journal.Model
{
    public class JournalModel
    {
        public readonly ObservableList<JournalEntry> journalEntries = new ObservableList<JournalEntry>();

        /// <summary>
        /// Add a Journal Entry to the Journal Model
        /// </summary>
        public void Add(JournalEntry journalEntry) => journalEntries.Add(journalEntry);

        /// <summary>
        /// Unlock the Journal Entry associated with the Journal Data
        /// </summary>
        public void Unlock(JournalData data)
        {
            // Get the first Journal Entry associated with the Journal Data
            JournalEntry entry = journalEntries.First(entry => entry.Data == data);

            // Set the Journal Entry to unlocked
            entry.Unlocked = true;

            // Invoek the Observable List
            journalEntries.Invoke();
        }
    }
}

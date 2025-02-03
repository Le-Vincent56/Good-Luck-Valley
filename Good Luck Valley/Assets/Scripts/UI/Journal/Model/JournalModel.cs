using GoodLuckValley.Architecture.ObservableList;
using System.Linq;

namespace GoodLuckValley.UI.Journal.Model
{
    public class JournalModel
    {
        private readonly ObservableList<JournalEntry> journalEntries = new ObservableList<JournalEntry>();

        public ObservableList<JournalEntry> JournalEntries { get => journalEntries; }

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

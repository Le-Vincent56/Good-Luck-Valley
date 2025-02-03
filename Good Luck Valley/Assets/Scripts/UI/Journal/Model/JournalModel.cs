using GoodLuckValley.Architecture.ObservableList;

namespace GoodLuckValley.UI.Journal.Model
{
    public class JournalModel
    {
        public readonly ObservableList<JournalEntry> journalEntries = new ObservableList<JournalEntry>();

        /// <summary>
        /// Add a Journal Entry to the Journal Model
        /// </summary>
        public void Add(JournalEntry journalEntry) => journalEntries.Add(journalEntry);
    }
}

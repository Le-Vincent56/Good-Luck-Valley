using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Journal
{
    [CreateAssetMenu(fileName = "Note Data", menuName = "Journal/Journal Entries Database")]
    public class JournalEntries : ScriptableObject
    {
        public List<JournalEntryData> Entries = new List<JournalEntryData>();
        public int Count { get => Entries.Count; }

        /// <summary>
        /// Sort the Journal entry database
        /// </summary>
        public void Sort() => Entries.Sort((note1, note2) => note1.ID.CompareTo(note2.ID));
    }
}
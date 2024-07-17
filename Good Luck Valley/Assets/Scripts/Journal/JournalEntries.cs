using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.Journal
{
    [CreateAssetMenu(fileName = "Note Data", menuName = "Journal/Journal Entries Database")]
    public class JournalEntries : ScriptableObject
    {
        public List<JournalEntryData> Entries = new List<JournalEntryData>();
        public int Count { get => Entries.Count; }
    }
}
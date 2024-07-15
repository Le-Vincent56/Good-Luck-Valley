using System.Collections.Generic;
using UnityEngine;
using GoodLuckValley.Patterns.Singletons;

namespace GoodLuckValley.Journal
{
    public class Journal : PersistentSingleton<Journal>
    {
        [SerializeField] private JournalEntries entriesDatabase;
        [SerializeField] private List<NoteData> unlockedEntries;
        [SerializeField] private int unlockedNum;
        [SerializeField] private bool unlocked;

        public bool Unlocked { get => unlocked; }

        public void Unlock() => unlocked = true;

        /// <summary>
        /// Unlock the next Journal entry
        /// </summary>
        public void GetNextNote()
        {
            // If the unlocked number is greater than the database count, return
            if (unlockedNum >= entriesDatabase.Count) return;

            // Add the note from the database to the unlocked entries
            unlockedEntries.Add(entriesDatabase.Notes[unlockedNum]);

            // Print the note
            entriesDatabase.Notes[unlockedNum].Print();

            // Increment the number
            unlockedNum++;
        }

        /// <summary>
        /// Get the currently unlocked entries
        /// </summary>
        /// <returns>The List of NoteDatas representing the Journal entries</returns>
        public List<NoteData> GetCurrentEntries() => unlockedEntries;
    }
}
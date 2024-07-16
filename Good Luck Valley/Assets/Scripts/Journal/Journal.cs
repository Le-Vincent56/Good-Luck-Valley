using System.Collections.Generic;
using UnityEngine;
using GoodLuckValley.Patterns.Singletons;
using GoodLuckValley.Events;

namespace GoodLuckValley.Journal
{
    public class Journal : PersistentSingleton<Journal>
    {
        [Header("Events")]
        [SerializeField] private GameEvent onNewJournalEntryUnlocked;

        [Header("References")]
        [SerializeField] private JournalEntries entriesDatabase;

        [Header("Fields")]
        [SerializeField] private List<JournalEntry> unlockedEntries;
        [SerializeField] private JournalEntry journalEntryInProgress;
        [SerializeField] private JournalEntry lastOpenedJournalEntry;
        [SerializeField] private int notesCollectedNum;
        [SerializeField] private int journalEntriesUnlocked;
        [SerializeField] private bool unlocked;

        public bool Unlocked { get => unlocked; }

        protected override void Awake()
        {
            base.Awake();

            unlockedEntries = new List<JournalEntry>();
        }

        public void Unlock()
        {
            // Set unlocked to true
            unlocked = true;
            
            // Set the Journal entries unlocked to 0
            journalEntriesUnlocked = 0;

            // Set the notes collected number to 1 (counts as the first note)
            notesCollectedNum = 1;

            // Set the current journal entry
            journalEntryInProgress = new JournalEntry(
                    entriesDatabase.Entries[journalEntriesUnlocked].Title,
                    entriesDatabase.Entries[journalEntriesUnlocked].TotalContent[0]
            );

            // Add the first Journal entry from the database
            unlockedEntries.Add(journalEntryInProgress);

            // Update the journal entry progress
            journalEntryInProgress.UpdateProgress(notesCollectedNum);
        }

        /// <summary>
        /// Get the next Note
        /// </summary>
        public void GetNextNote()
        {
            // Exit case - if the unlocked number is greater than the database count
            if (journalEntriesUnlocked > entriesDatabase.Count) return;

            // Check if the current entry has been completed
            if (journalEntryInProgress.Completed)
            {
                // Reset the number of notes collected
                notesCollectedNum = 1;

                // Unlock the next Journal entry
                UnlockNextJournalEntry();

                // Exit early
                return;
            }

            // Add the note from the database to the unlocked entries
            journalEntryInProgress.AddContent(
                entriesDatabase.Entries[journalEntriesUnlocked]
                    .TotalContent[notesCollectedNum]
            );

            // Increment the number
            notesCollectedNum++;

            // Update entry progress
            journalEntryInProgress.UpdateProgress(notesCollectedNum);
        }

        /// <summary>
        /// Unlock the next Journal entry
        /// </summary>
        private void UnlockNextJournalEntry()
        {
            // Increment the number of Journal entries unlocked
            journalEntriesUnlocked++;

            // Exit case - if the number of journal entries unlocked is greater than the database count
            if (journalEntriesUnlocked >= entriesDatabase.Count) return;

            // Set the current content to the number of notes collected (should be 0)

            journalEntryInProgress = new JournalEntry(
                    entriesDatabase.Entries[journalEntriesUnlocked].Title,
                    entriesDatabase.Entries[journalEntriesUnlocked].TotalContent[0]
            );

            // Add the newly unlocked Journal entry
            unlockedEntries.Add(journalEntryInProgress);

            // Update the progress of the current journal entry
            journalEntryInProgress.UpdateProgress(notesCollectedNum);

            //onNewJournalEntryUnlocked.Raise(this, null);
        }

        /// <summary>
        /// Get the currently unlocked entries
        /// </summary>
        /// <returns>The List of NoteDatas representing the Journal entries</returns>
        public List<JournalEntry> GetCurrentEntries() => unlockedEntries;

        /// <summary>
        /// Get the Journal entry in progress
        /// </summary>
        /// <returns></returns>
        public JournalEntry GetProgressingEntry() => journalEntryInProgress;

        /// <summary>
        /// Get a Journal entry at a specific index
        /// </summary>
        /// <param name="index">The index to retrieve a JournalEntry at</param>
        /// <returns>An unlocked JournalEntry at the given index</returns>
        public JournalEntry GetEntryAt(int index) => unlockedEntries[index];

        /// <summary>
        /// Get the index of the entry in progress
        /// </summary>
        /// <returns></returns>
        public int GetCurrentIndex() => unlockedEntries.IndexOf(journalEntryInProgress);

        /// <summary>
        /// Get the number of notes collected
        /// </summary>
        /// <returns></returns>
        public int GetNumOfNotesCollected() => notesCollectedNum;

        /// <summary>
        /// Get the number of entries unlocked
        /// </summary>
        /// <returns></returns>
        public int GetNumOfUnlockedEntries() => journalEntriesUnlocked;

        /// <summary>
        /// Set the last opened Journal entry
        /// </summary>
        /// <param name="journalEntry">The last opened Journal Entry</param>
        public void SetLastOpenedEntry(JournalEntry journalEntry) => lastOpenedJournalEntry = journalEntry;
    }
}
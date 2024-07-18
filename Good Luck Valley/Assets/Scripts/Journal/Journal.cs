using System.Collections.Generic;
using UnityEngine;
using GoodLuckValley.Patterns.Singletons;
using GoodLuckValley.Events;
using GoodLuckValley.UI.Notifications;

namespace GoodLuckValley.Journal
{
    public class Journal : PersistentSingleton<Journal>
    {
        [Header("Events")]
        [SerializeField] private GameEvent onNotify;

        [Header("References")]
        [SerializeField] private JournalEntries entriesDatabase;

        [Header("Fields")]
        [SerializeField] private List<JournalEntry> unlockedEntries;
        [SerializeField] private JournalEntry journalEntryInProgress;
        [SerializeField] private JournalEntry lastOpenedJournalEntry;
        [SerializeField] private int notesCollectedNum;
        [SerializeField] private int journalEntriesUnlocked;
        [SerializeField] private bool unlocked;
        [SerializeField] private bool notificationInput = false;
        [SerializeField] private bool open;

        public bool Unlocked { get => unlocked; }
        public bool NotificationInput { get => notificationInput; }
        public bool Open { get => open; }

        protected override void Awake()
        {
            base.Awake();

            unlockedEntries = new List<JournalEntry>();
        }

        /// <summary>
        /// Unlock the Journal and set default values
        /// </summary>
        public void Unlock()
        {
            // Set unlocked to true
            unlocked = true;
            
            // Set the Journal Entries unlocked to 0
            journalEntriesUnlocked = 0;

            // Set the Notes collected number to 1 (counts as the first note)
            notesCollectedNum = 1;

            // Set the current Journal Entry
            journalEntryInProgress = new JournalEntry(
                    entriesDatabase.Entries[journalEntriesUnlocked].Title,
                    entriesDatabase.Entries[journalEntriesUnlocked].TotalContent[0]
            );

            // Add the first Journal Entry from the database
            unlockedEntries.Add(journalEntryInProgress);

            // Update the Journal Entry progress
            journalEntryInProgress.UpdateProgress(notesCollectedNum);

            // Send a Journal pickup notification
            onNotify.Raise(this, new NotificationData(NotificationHandler.Type.Journal, GetCurrentIndex()));

            // Set the last opened Journal Entry to the one in progress
            lastOpenedJournalEntry = journalEntryInProgress;
        }

        /// <summary>
        /// Get the next Note
        /// </summary>
        public void GetNextNote()
        {
            // Exit case - if the unlocked number is greater than the database count
            if (journalEntriesUnlocked > entriesDatabase.Count) return;

            // Check if the current Journal Entry has been completed
            if (journalEntryInProgress.Completed)
            {
                // Reset the number of Notes collected
                notesCollectedNum = 1;

                // Unlock the next Journal Entry
                UnlockNextJournalEntry();

                // Exit early
                return;
            }

            // Add the Note from the database to the unlocked Journal Entries
            journalEntryInProgress.AddContent(
                entriesDatabase.Entries[journalEntriesUnlocked]
                    .TotalContent[notesCollectedNum]
            );

            // Increment the number of Notes collected
            notesCollectedNum++;

            // Update Journal Entry progress
            journalEntryInProgress.UpdateProgress(notesCollectedNum);

            // Check if the Journal Entry has been completed
            if(journalEntryInProgress.Completed)
            {
                // Send a Journal Entry notification
                onNotify.Raise(this, new NotificationData(NotificationHandler.Type.Entry, GetCurrentIndex()));
            } else
            {
                // Send a Note notification
                onNotify.Raise(this, new NotificationData(NotificationHandler.Type.Note, GetCurrentIndex()));
            }
        }

        /// <summary>
        /// Unlock the next Journal entry
        /// </summary>
        private void UnlockNextJournalEntry()
        {
            // Increment the number of Journal Entries unlocked
            journalEntriesUnlocked++;

            // Exit case - if the number of Journal Entries unlocked is greater than the database count
            if (journalEntriesUnlocked >= entriesDatabase.Count) return;

            // Set the current content to the number of Notes collected (should be 0)
            journalEntryInProgress = new JournalEntry(
                    entriesDatabase.Entries[journalEntriesUnlocked].Title,
                    entriesDatabase.Entries[journalEntriesUnlocked].TotalContent[0]
            );

            // Add the newly unlocked Journal Entry
            unlockedEntries.Add(journalEntryInProgress);

            // Update the progress of the current Journal Entry
            journalEntryInProgress.UpdateProgress(notesCollectedNum);

            // Send a Note notification
            onNotify.Raise(this, new NotificationData(NotificationHandler.Type.Note, GetCurrentIndex()));
        }

        /// <summary>
        /// Get the currently unlocked entries
        /// </summary>
        /// <returns>The List of NoteDatas representing the Journal entries</returns>
        public List<JournalEntry> GetCurrentEntries() => unlockedEntries;

        /// <summary>
        /// Get the Journal Entry in progress
        /// </summary>
        /// <returns></returns>
        public JournalEntry GetProgressingEntry() => journalEntryInProgress;

        /// <summary>
        /// Get a Journal Entry at a specific index
        /// </summary>
        /// <param name="index">The index to retrieve a JournalEntry at</param>
        /// <returns>An unlocked JournalEntry at the given index</returns>
        public JournalEntry GetEntryAt(int index)
        {
            // If not unlocked, return null
            if (!unlocked) return null;

            return unlockedEntries[index];
        }

        /// <summary>
        /// Get the index of the Entry in progress
        /// </summary>
        /// <returns>The index of the current Journal Entry in progress</returns>
        public int GetCurrentIndex() => unlockedEntries.IndexOf(journalEntryInProgress);

        /// <summary>
        /// Get the index of the last opened Journal Entry
        /// </summary>
        /// <returns>The index of the last opened Journal Entry</returns>
        public int GetLastOpenedIndex() => unlockedEntries.IndexOf(lastOpenedJournalEntry);

        /// <summary>
        /// Get the number of Notes collected
        /// </summary>
        /// <returns></returns>
        public int GetNumOfNotesCollected() => notesCollectedNum;

        /// <summary>
        /// Get the number of Journal Entries unlocked
        /// </summary>
        /// <returns>The number of unlocked Journal Entries</returns>
        public int GetNumOfUnlockedEntries() => journalEntriesUnlocked;

        /// <summary>
        /// Set whether or not the Journal is unlocked
        /// </summary>
        /// <param name="unlocked">True if the Journal is unlocked, false if not</param>
        public void SetUnlocked(bool unlocked) => this.unlocked = unlocked;

        /// <summary>
        /// Set the Journal Entry in progress
        /// </summary>
        /// <param name="index">The index of the Journal Entry in progress</param>
        public void SetProgressingEntry(int index) => journalEntryInProgress = GetEntryAt(index);

        /// <summary>
        /// Set whether or not the Journal is open
        /// </summary>
        /// <param name="open">True if the Journal is open, false if not</param>
        public void SetJournalOpen(bool open) => this.open = open;

        /// <summary>
        /// Set the last opened Journal Entry
        /// </summary>
        /// <param name="journalIndex">The index of the last opened Journal Entry</param>
        public void SetLastOpenedEntry(int journalIndex) => lastOpenedJournalEntry = GetEntryAt(journalIndex);

        /// <summary>
        /// Set the number of Notes collected
        /// </summary>
        /// <param name="notesCollectedNum">The number of Notes collected</param>
        public void SetNotesCollectedNum(int notesCollectedNum) => this.notesCollectedNum = notesCollectedNum;

        /// <summary>
        /// Set the number of Journal Entries that have been unlocked
        /// </summary>
        /// <param name="journalEntriesUnlocked">The number of Journal Entries that have been unlocked</param>
        public void SetEntriesUnlockedNum(int journalEntriesUnlocked) => this.journalEntriesUnlocked = journalEntriesUnlocked;

        /// <summary>
        /// Set the notification input
        /// </summary>
        /// <param name="notificationInput">True if notification input is enabled, false if not</param>
        public void SetNotificationInput(bool notificationInput) => this.notificationInput = notificationInput;
    }
}
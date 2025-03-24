using GoodLuckValley.Events;
using GoodLuckValley.Events.Journal;
using GoodLuckValley.Persistence;
using GoodLuckValley.UI.Journal.Model;
using System.Collections.Generic;
using UnityEngine;

namespace GoodLuckValley.UI.Journal.Persistence
{
    public class JournalSaveHandler : MonoBehaviour, IBind<GoodLuckValley.Persistence.JournalData>
    {
        private JournalSystem system;
        private bool unlocked;
        private List<int> entriesUnlocked;

        [SerializeField] private GoodLuckValley.Persistence.JournalData journalData;

        private EventBinding<UnlockJournal> onUnlockJournal;

        [field: SerializeField] public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid();

        private void OnEnable()
        {
            onUnlockJournal = new EventBinding<UnlockJournal>(UpdateUnlockData);
            EventBus<UnlockJournal>.Register(onUnlockJournal);
        }

        private void OnDisable()
        {
            EventBus<UnlockJournal>.Deregister(onUnlockJournal);
        }

        /// <summary>
        /// Update the Journal Unlock data
        /// </summary>
        private void UpdateUnlockData()
        {
            unlocked = true;
            journalData.Unlocked = unlocked;
        }

        /// <summary>
        /// Update the Journal Entry data
        /// </summary>
        private void UpdateEntryData(IList<JournalEntry> entries)
        {
            // Clear the Entries Unlocked List
            entriesUnlocked.Clear();

            // Iterate through each Journal Entry
            for(int i = 0; i < entries.Count; i++)
            {
                // Skip if the Journal Entry is not unlocked
                if (!entries[i].Unlocked) continue;

                // Add the Entry Index to the Entries Unlocked List
                entriesUnlocked.Add(i);
            }

            // Set the save data
            journalData.EntriesUnlocked = entriesUnlocked;
        }

        /// <summary>
        /// Bind Journal data for persistence
        /// </summary>
        public void Bind(GoodLuckValley.Persistence.JournalData journalData)
        {
            // Bind data
            this.journalData = journalData;
            this.journalData.ID = ID;

            // Set data
            unlocked = journalData.Unlocked;
            entriesUnlocked = journalData.EntriesUnlocked;

            // Get components
            system = GetComponent<JournalSystem>();

            // Unlock Journal Entries
            system.UnlockEntries(journalData.EntriesUnlocked);

            // Add an event to update the save data when any Journal Entry changes
            system.Controller.Model.JournalEntries.AnyValueChanged += UpdateEntryData;

            // Update the Journal Unlocked status
            EventBus<UpdateJournalUnlock>.Raise(new UpdateJournalUnlock()
            {
                Unlocked = journalData.Unlocked
            });

            // Exit case - if the Journal is not unlocked
            if (!unlocked) return;

            // Unlock the Journal
            system.Unlock();
        }
    }
}

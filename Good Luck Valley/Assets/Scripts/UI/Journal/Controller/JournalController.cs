using GoodLuckValley.Audio;
using GoodLuckValley.UI.Journal.Model;
using GoodLuckValley.UI.Journal.View;
using GoodLuckValley.Utilities.Preconditions;
using System.Collections.Generic;

namespace GoodLuckValley.UI.Journal.Controller
{
    public class JournalController
    {
        public class Builder
        {
            private JournalModel model;
            private JournalSFX sfx;

            /// <summary>
            /// Build with initial Journal Datas
            /// </summary>
            public Builder WithJournalEntries(JournalData[] journalDatas)
            {
                // Create a new Journal Model
                model = new JournalModel();

                // Iterate through each Journal Data
                foreach (JournalData journalData in journalDatas)
                {
                    // Add a Journal Entry with the associated Journal Data
                    // to the Journal Model
                    model.Add(new JournalEntry(journalData));
                }

                return this;
            }

            /// <summary>
            /// Build with SFX
            /// </summary>
            public Builder WithSFX(JournalSFX sfx)
            {
                this.sfx = sfx;

                return this;
            }

            /// <summary>
            /// Build the Journal Controller
            /// </summary>
            public JournalController Build(JournalView view)
            {
                // Ensure that the Journal View is not null
                Preconditions.CheckNotNull(view);

                return new JournalController(model, view, sfx);
            }
        }

        private readonly JournalModel model;
        private readonly JournalView view;
        private readonly JournalSFX sfx;
        private bool open;
        private bool unlocked;
        private bool fromPause;

        public JournalModel Model { get => model; }
        public JournalView View { get => view; }
        public bool Open { get => open; set => open = value; }

        public JournalController(JournalModel model, JournalView view, JournalSFX sfx)
        {
            this.model = model;
            this.view = view;
            this.sfx = sfx;

            open = false;

            // Connect the Model and the View
            ConnectModel();
            ConnectView();
        }

        /// <summary>
        /// Connect the Journal Model to the Journal Controller
        /// </summary>
        private void ConnectModel()
        {
            model.JournalEntries.AnyValueChanged += UpdateEntries;
        }

        /// <summary>
        /// Connect the Journal View to the Journal Controller
        /// </summary>
        private void ConnectView()
        {
            // Iterate through each View
            for(int i = 0; i < view.Entries.Count; i++)
            {
                // Register events
                view.Entries[i].RegisterListener(OnEntryClicked);
            }

            // Iterate through each Tab
            for(int i = 0; i < view.Tabs.Count; i++)
            {
                // Register events
                view.Tabs[i].RegisterListener(OnTabClicked);
            }

            // Update the Entry Buttons
            view.UpdateEntries(model.JournalEntries);
        }

        /// <summary>
        /// Update the Journal View Entry Buttons
        /// </summary>
        private void UpdateEntries(IList<JournalEntry> entries) => view.UpdateEntries(entries);

        /// <summary>
        /// Handle a Tab being clicked
        /// </summary>
        private void OnTabClicked(View.TabButton button)
        {
            view.ShowTabEntries(button.Tab);
        }

        /// <summary>
        /// Handle an Entry being clicked
        /// </summary>
        private void OnEntryClicked(EntryButton button)
        {
            // Play the entry selected sound
            sfx.EntrySelected(button.Empty);

            view.ShowEntryContent(button);
        }

        /// <summary>
        /// Open the Journal View
        /// </summary>
        public bool OpenJournal(bool fromPause = false)
        {
            // Exit case - the Journal is not unlocked
            if (!unlocked) return false;

            // Exit case - if already open
            if (open) return false;

            // Set whether or not the Player is coming from the pause menu
            this.fromPause = fromPause;

            // Play the open journal sound
            sfx.Open();

            // Show the Journal View
            view.Show(this);

            return true;
        }

        /// <summary>
        /// Close the Journal View
        /// </summary>
        public void CloseJournal()
        {
            // Exit case - if not already open
            if (!open) return;

            // Play the close journal sound
            sfx.Close();

            // Hide the Journal View
            view.Hide(this, fromPause);
        }

        /// <summary>
        /// Select the last selected Tab
        /// </summary>
        public void SelectLastTab() => view.SelectLastTab();

        /// <summary>
        /// Select the last selected Entry
        /// </summary>
        public void SelectLastEntry()
        {
            // Select the last selected Entry
            view.SelectLastEntry();

            // Correct tabs
            view.CorrectTabs(view.LastSelectedEntry.Tab);
        }

        /// <summary>
        /// Unlock the Journal
        /// </summary>
        public void Unlock() => unlocked = true;

        /// <summary>
        /// Unlock a Journal Entry in the Journal Model using a Journal Data
        /// </summary>
        public void UnlockEntry(JournalData dataToAdd) => model.Unlock(dataToAdd);

        /// <summary>
        /// Unlock a Journal Entry in the Journal Model using an index
        /// </summary>
        public void UnlockEntry(int index) => model.Unlock(index);
    }
}

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
            private readonly JournalModel model = new JournalModel();

            /// <summary>
            /// Build with initial Journal Datas
            /// </summary>
            public Builder WithJournalEntries(JournalData[] journalDatas)
            {
                // Iterate through each Journal Data
                foreach(JournalData journalData in journalDatas)
                {
                    // Add a Journal Entry with the associated Journal Data
                    // to the Journal Model
                    model.Add(new JournalEntry(journalData));
                }

                return this;
            }

            /// <summary>
            /// Build the Journal Controller
            /// </summary>
            public JournalController Build(JournalView view)
            {
                // Ensure that the Journal View is not null
                Preconditions.CheckNotNull(view);

                return new JournalController(model, view);
            }
        }

        private readonly JournalModel model;
        private readonly JournalView view;
        private bool unlocked;
        private bool fromPause;

        public JournalModel Model { get => model; }
        public JournalView View { get => view; }

        public JournalController(JournalModel model, JournalView view)
        {
            this.model = model;
            this.view = view;

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
            for(int i = 0; i < view.Entries.Length; i++)
            {
                // Register events
                view.Entries[i].RegisterListener(OnEntryClicked);
            }

            // Iterate through each Tab
            for(int i = 0; i < view.Tabs.Length; i++)
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
            view.ShowEntryContent(button.TitleText, button.Content);
        }

        /// <summary>
        /// Open the Journal View
        /// </summary>
        public bool Open(bool fromPause = false)
        {
            // Exit case - the Journal is not unlocked
            if (!unlocked) return false;

            // Set whether or not the Player is coming from the pause menu
            this.fromPause = fromPause;

            // Show the Journal View
            view.Show();

            return true;
        }

        /// <summary>
        /// Close the Journal View
        /// </summary>
        public void Close() => view.Hide(fromPause);

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

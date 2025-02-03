using GoodLuckValley.UI.Journal.Model;
using GoodLuckValley.UI.Journal.View;
using GoodLuckValley.Utilities.Preconditions;
using System.Collections.Generic;
using UnityEngine;

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
            model.journalEntries.AnyValueChanged += UpdateEntries;
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
            view.UpdateEntries(model.journalEntries);
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
        }

        /// <summary>
        /// Open the Journal View
        /// </summary>
        public void Open() => view.Show();

        /// <summary>
        /// Close the Journal View
        /// </summary>
        public void Close() => view.Hide();

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
    }
}

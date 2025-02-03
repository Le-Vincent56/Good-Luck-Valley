using GoodLuckValley.Events;
using GoodLuckValley.Events.Journal;
using GoodLuckValley.Input;
using GoodLuckValley.UI.Journal.Controller;
using GoodLuckValley.UI.Journal.Model;
using GoodLuckValley.UI.Journal.View;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GoodLuckValley.UI.Journal
{
    public class JournalSystem : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameInputReader gameInputReader;
        [SerializeField] private UIInputReader uiInputReader;
        private JournalController controller;
        [SerializeField] private JournalView view;

        [Header("Data")]
        [SerializeField] private JournalData[] journalDatas;

        private EventBinding<UnlockJournal> onUnlockJournal;
        private EventBinding<UnlockJournalEntry> onAddJournalEntry;

        private void OnEnable()
        {
            gameInputReader.Journal += OpenJournal;
            uiInputReader.Cancel += CloseJournal;
            uiInputReader.Navigate += EnterTabs;
            uiInputReader.Navigate += TabNavigation;

            onUnlockJournal = new EventBinding<UnlockJournal>(Unlock);
            EventBus<UnlockJournal>.Register(onUnlockJournal);

            onAddJournalEntry = new EventBinding<UnlockJournalEntry>(UnlockEntry);
            EventBus<UnlockJournalEntry>.Register(onAddJournalEntry);
        }

        private void OnDisable()
        {
            gameInputReader.Journal -= OpenJournal;
            uiInputReader.Cancel -= CloseJournal;
            uiInputReader.Navigate -= EnterTabs;
            uiInputReader.Navigate -= TabNavigation;

            EventBus<UnlockJournal>.Deregister(onUnlockJournal);
            EventBus<UnlockJournalEntry>.Deregister(onAddJournalEntry);
        }

        private void Start()
        {
            // Build the Journal Controller with the data and the view
            controller = new JournalController.Builder()
                .WithJournalEntries(journalDatas)
                .Build(view);
        }

        /// <summary>
        /// Event callback to handle entering the Tabs section from the Entry section
        /// </summary>
        private void EnterTabs(Vector2 input)
        {
            // Exit case - there was no movement
            if (input == Vector2.zero) return;

            // Exit case - the currently selected Game Object is not an Entry Button
            if (!EventSystem.current.currentSelectedGameObject.TryGetComponent(out EntryButton entryButton)) return;

            // Exit case - if not attempting to enter the Tabs section
            if ((int)input.x != -1) return;

            // Select the last selected Tab
            controller.SelectLastTab();
        }

        /// <summary>
        /// Event callback to handle returning to the Entry section from the Tab section
        /// </summary>
        private void TabNavigation(Vector2 input)
        {
            // Exit case - there was no movement
            if (input == Vector2.zero) return;

            // Exit case - the currently selected Game Object does not have a Tab Button
            if (!EventSystem.current.currentSelectedGameObject.TryGetComponent(out View.TabButton hoveredTab)) return;

            // Get the y-direction of movement
            int yDirection = -(int)input.y;

            // Check if the y-direction is not 0
            if(yDirection != 0)
            {
                // Set the default current index
                int currentIndex = 0;

                // Iterate through each Tab
                for(int i = 0; i < view.Tabs.Length; i++)
                {
                    // Check if the Tab equals the Hovered Tab
                    if(view.Tabs[i] == hoveredTab)
                    {
                        // Get the current index
                        currentIndex = i;
                        break;
                    }
                }

                // Default a Tab to Select
                View.TabButton tabToSelect = null;

                // Loop until a Tab is selected
                while (tabToSelect == null)
                {
                    // Get the next index
                    currentIndex += yDirection;

                    // Check if the index is out of bounds
                    if (currentIndex < 0 || currentIndex >= view.Tabs.Length)
                    {
                        // Set the index to the opposite end
                        currentIndex = currentIndex < 0 ? view.Tabs.Length - 1 : 0;
                    }

                    // Check if the tab to select is not interactable
                    if (!view.Tabs[currentIndex].Interactable)
                    {
                        // Skip over it
                        currentIndex += yDirection;
                    }

                    // Check if the index is out of bounds
                    if (currentIndex < 0 || currentIndex >= view.Tabs.Length)
                    {
                        // Set the index to the opposite end
                        currentIndex = currentIndex < 0 ? view.Tabs.Length - 1 : 0;
                    }

                    // Check if the Tab is interactable
                    if (view.Tabs[currentIndex].Interactable) 
                        // If so, select the Tab
                        tabToSelect = view.Tabs[currentIndex];
                }

                // Select the tab
                EventSystem.current.SetSelectedGameObject(tabToSelect.gameObject);

                return;
            }

            // Exit case - if not attempting to return to the Entry section
            if ((int)input.x != 1) return;

            // Select the lat selected Entry
            controller.SelectLastEntry();
        }

        /// <summary>
        /// Event callback to open the Journal
        /// </summary>
        private void OpenJournal(bool started)
        {
            // Exit case - if opening the Journal fails
            if (!controller.Open()) return;

            // Enable UI input
            gameInputReader.Disable();
            uiInputReader.Enable();
        }

        /// <summary>
        /// Event callback to close the Journal
        /// </summary>
        private void CloseJournal(bool started)
        {
            // Close the Journal
            controller.Close();

            // Enable game input
            uiInputReader.Disable();
            gameInputReader.Enable();
        }

        /// <summary>
        /// Unlock the Journal
        /// </summary>
        private void Unlock() => controller.Unlock();

        /// <summary>
        /// Add an Entry to the Journal
        /// </summary>
        private void UnlockEntry(UnlockJournalEntry eventData) => controller.UnlockEntry(eventData.Data);
    }
}

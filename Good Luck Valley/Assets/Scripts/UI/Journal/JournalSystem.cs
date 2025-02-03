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

        private void OnEnable()
        {
            gameInputReader.Journal += OpenJournal;
            uiInputReader.Cancel += CloseJournal;
            uiInputReader.Navigate += EnterTabs;
            uiInputReader.Navigate += ReturnToEntry;
        }

        private void OnDisable()
        {
            gameInputReader.Journal -= OpenJournal;
            uiInputReader.Cancel -= CloseJournal;
            uiInputReader.Navigate -= EnterTabs;
            uiInputReader.Navigate -= ReturnToEntry;
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
        private void ReturnToEntry(Vector2 input)
        {
            // Exit case - there was no movement
            if (input == Vector2.zero) return;

            // Exit case - the currently selected Game Object does not have a Tab Button
            if (!EventSystem.current.currentSelectedGameObject.TryGetComponent(out View.TabButton tab)) return;

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
            // Open the Journal
            controller.Open();

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
    }
}

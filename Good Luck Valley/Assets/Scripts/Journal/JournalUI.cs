using GoodLuckValley.Events;
using GoodLuckValley.Player.Input;
using GoodLuckValley.UI;
using GoodLuckValley.UI.Notifications;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.Journal.UI
{
    public class JournalUI : FadePanel
    {
        [Header("Events")]
        [SerializeField] private GameEvent onOpenJournal;
        [SerializeField] private GameEvent onHideJournal;
        [SerializeField] private GameEvent onSetJournalInputAction;
        [SerializeField] private GameEvent onSetDefaultInputAction;
        [SerializeField] private GameEvent onSetPauseInputAction;

        [Header("References")]
        [SerializeField] private InputReader defaultInputReader;
        [SerializeField] private JournalInputReader journalInputReader;
        [SerializeField] private PauseInputReader pauseInputReader;
        [SerializeField] private Image journalImage;
        [SerializeField] private InspectWindow inspectWindow;

        [Header("Fields")]
        [SerializeField] private bool fromPauseMenu;
        [SerializeField] private bool escToClose;
        [SerializeField] private bool inspectWindowOpen;
        [SerializeField] private int journalPageIndex;
        [SerializeField] private int journalPageUpperBound;
        [SerializeField] private int journalPageHighestBound;

        [Space(5f)]
        [SerializeField] private List<Sprite> journalSprites;
        private Dictionary<string, int> spriteHashTable = new Dictionary<string, int>();

        protected override void Awake()
        {
            base.Awake();

            // Get the Inspect Window
            inspectWindow = GetComponentInChildren<InspectWindow>();

            // Create the Sprite Hash Table
            spriteHashTable = new Dictionary<string, int>()
            {
                { "J_0_H_P1", 0 },
                { "J_0_H_P2", 1 },
                { "J_0_H_P3", 2 },
                { "J_1_H_P1", 3 },
                { "J_1_H_P2", 4 },
                { "J_1_H_P3", 5 },
                { "J_1_H_FIRST", 6 },
                { "J_1_H_MIDDLE", 7 },
                { "J_2_H_P1", 8 },
                { "J_2_H_P2", 9 },
                { "J_2_H_P3", 10 },
                { "J_2_H_FIRST", 11 },
                { "J_2_H_MIDDLE", 12 },
            };
        }

        private void OnEnable()
        {
            defaultInputReader.OpenJournal += OpenJournalInput;
            journalInputReader.Back += Back;
            journalInputReader.Read += Read;
            journalInputReader.NextPage += NextPage;
            journalInputReader.PrevPage += PrevPage;
            pauseInputReader.OpenJournal += OpenJournalMenuInput;
        }

        private void OnDisable()
        {
            defaultInputReader.OpenJournal -= OpenJournalInput;
            journalInputReader.Back -= Back;
            journalInputReader.Read -= Read;
            journalInputReader.NextPage -= NextPage;
            journalInputReader.PrevPage -= PrevPage;
            pauseInputReader.OpenJournal -= OpenJournalMenuInput;
        }

        private void Start()
        {
            // Set the default input action
            onSetDefaultInputAction.Raise(this, null);
        }

        /// <summary>
        /// Open the Journal UI using input
        /// </summary>
        public void OpenJournalInput(bool started, bool performed)
        {
            // Exit case - the button hasn't been lifted
            if (started) return;

            // Exit case - the Journal has not been unlocked
            if (!Journal.Instance.Unlocked) return;

            // Exit case - if notification input is enabled
            if (Journal.Instance.NotificationInput) return;

            // Pause the game if not paused already
            if (!PauseManager.Instance.IsSoftPaused)
            {
                // Open the journal
                // Calls to:
                //  - PauseMenu.ToggleSoftPause();
                onOpenJournal.Raise(this, null);
            }

            // Notify that the Journal was not opened from the pause menu
            fromPauseMenu = false;

            // Set the Journal input action map
            // Calls to:
            //  - PlayerInputHandler.EnableJournalInput()
            onSetJournalInputAction.Raise(this, null);

            // Set pressing "Escape" to close to true
            escToClose = true;

            // Get the last opened index
            int lastOpenedIndex = Journal.Instance.GetLastOpenedIndex();

            // Update the Journal sprite
            UpdateJournalSprite(lastOpenedIndex);

            // Set the Journal Entry to read
            inspectWindow.SetEntry(Journal.Instance.GetEntryAt(lastOpenedIndex));

            // Show the UI
            ShowUI();

            // Set the Journal to open
            Journal.Instance.SetJournalOpen(true);
        }

        /// <summary>
        /// Open the Journal UI from the Menu using the button
        /// </summary>
        public void OpenJournalMenu(Component sender, object data)
        {
            // Return if the Journal is not unlocked
            if (!Journal.Instance.Unlocked) return;

            // Notify that the Journal was opened from the pause menu
            fromPauseMenu = true;

            // Calls to:
            //  - PlayerInputHandler.EnableJournalInput()
            onSetJournalInputAction.Raise(this, null);

            // Set pressing "Escape" to close to true
            escToClose = true;

            // Get the last opened index
            int lastOpenedIndex = Journal.Instance.GetLastOpenedIndex();

            // Update the Journal sprite
            UpdateJournalSprite(lastOpenedIndex);

            // Set the Journal Entry to read
            inspectWindow.SetEntry(Journal.Instance.GetEntryAt(lastOpenedIndex));

            // Show the UI
            ShowUI();

            // Set the Journal to open
            Journal.Instance.SetJournalOpen(true);
        }

        /// <summary>
        /// Open the Journal UI from the Menu and using input
        /// </summary>
        /// <param name="started"></param>
        public void OpenJournalMenuInput(bool started)
        {
            // Exit case - the button hasn't been lifted
            if (started) return;

            // Exit case - the Journal has not yet been unlocked
            if (!Journal.Instance.Unlocked) return;

            // Notify that the Journal was opened from the pause menu
            fromPauseMenu = true;

            // Calls to:
            //  - PlayerInputHandler.EnableJournalInput()
            onSetJournalInputAction.Raise(this, null);

            // Set pressing "Escape" to close to true
            escToClose = true;

            // Get the last opened index
            int lastOpenedIndex = Journal.Instance.GetLastOpenedIndex();

            // Update the Journal sprite
            UpdateJournalSprite(lastOpenedIndex);

            // Set the Journal Entry to read
            inspectWindow.SetEntry(Journal.Instance.GetEntryAt(lastOpenedIndex));

            // Show the UI
            ShowUI();

            // Set the Journal to open
            Journal.Instance.SetJournalOpen(true);
        }

        /// <summary>
        /// Open the Journal from a Notification
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="data"></param>
        public void OpenJournalFromNotification(Component sender, object data)
        {
            // Verify that the event is being received from the correct sender
            if (sender is not NotificationHandler || data is not int) return;

            int sentPageIndex = (int)data;

            // Notify that the Journal was not opened from the pause menu
            fromPauseMenu = false;

            // Calls to:
            //  - PlayerInputHandler.EnableJournalInput()
            onSetJournalInputAction.Raise(this, null);

            // Set pressing "Escape" to close to true
            escToClose = true;

            // Update the Journal sprite
            UpdateJournalSprite(sentPageIndex);
            
            // Set the Journal Entry to read
            inspectWindow.SetEntry(Journal.Instance.GetEntryAt(sentPageIndex));

            // Show the UI
            ShowUI();

            // Set the Journal to open
            Journal.Instance.SetJournalOpen(true);
        }

        /// <summary>
        /// Hide the Journal UI
        /// </summary>
        /// <param name="started"></param>
        public void HideJournal()
        {
            // If paused and not from the pause menu, unpause
            if (PauseManager.Instance.IsSoftPaused && !fromPauseMenu)
            {
                // Hide the journal
                // Calls to:
                //  - PauseMenu.ToggleSoftPause();
                onHideJournal.Raise(this, null);
            }

            // Check if exiting to the pause menu or straight back to the game
            if(fromPauseMenu)
            {
                // Calls to:
                //  - PlayerInputHandler.EnablePauseInput()
                onSetPauseInputAction.Raise(this, null);
            } else
            {
                // Calls to:
                //  - PlayerInputHandler.EnableDefaultInput()
                onSetDefaultInputAction.Raise(this, null);
            }

            // Set the last opened journal entry
            Journal.Instance.SetLastOpenedEntry(journalPageIndex);

            // Reset variables
            fromPauseMenu = false;
            escToClose = false;

            // Hide the UI
            HideUI();

            // Set the Journal to closed
            Journal.Instance.SetJournalOpen(false);
        }

        /// <summary>
        /// Handle "Back" input for the Journal
        /// </summary>
        /// <param name="started"></param>
        public void Back(bool started)
        {
            // Exit case - the button hasn't been lifted yet
            if (started) return;

            // Check if "Escape" will close the journal or not
            if (escToClose)
                // If so, close the journal
                HideJournal();
            else
                // Otherwise, close the inspect window
                CloseInspectWindow();

        }

        /// <summary>
        /// Handle "Read" input for the Journal
        /// </summary>
        /// <param name="started"></param>
        public void Read(bool started)
        {
            // Exit case - if the button hasn't been lifted
            if (started) return;

            // Check if the inspect window is already open
            if (inspectWindowOpen)
                // Close the inspect window
                CloseInspectWindow();
            else
                // Otherwise, open the inspect window
                OpenInspectWindow();
        }

        /// <summary>
        /// Open the Inspect Window
        /// </summary>
        public void OpenInspectWindow()
        {
            // Set pressing "Escape" to close to false,
            // as Escape now closes the inspect window
            escToClose = false;

            // Set the progressing entry
            inspectWindow.SetEntry(Journal.Instance.GetEntryAt(journalPageIndex));

            // Hide the inspect window
            inspectWindow.ShowUI();

            // Set inspect window open
            inspectWindowOpen = true;
        }

        /// <summary>
        /// Close the Inspect Window
        /// </summary>
        public void CloseInspectWindow()
        {
            // Hide the inspect window
            inspectWindow.HideUI();

            // Set pressing "Escape" to close to false,
            // as Escape now closes the Journal
            escToClose = true;

            // Set inspect window closed
            inspectWindowOpen = false;
        }

        /// <summary>
        /// Traverse to the next Journal page
        /// </summary>
        /// <param name="started"></param>
        public void NextPage(bool started)
        {
            // Exit case - if the button hasn't been lifted yet
            if (started) return;

            // Exit case - if trying to leave the upper bound of the Journal
            if (journalPageIndex == Journal.Instance.GetNumOfUnlockedEntries()) return;

            UpdateJournalSprite(journalPageIndex + 1);
            inspectWindow.SetEntry(Journal.Instance.GetEntryAt(journalPageIndex));
        }

        /// <summary>
        /// Traverse to the previous Journal page
        /// </summary>
        /// <param name="started"></param>
        public void PrevPage(bool started)
        {
            // Exit case - if the button hasn't been lifted yet
            if (started) return;

            // Exit case - if trying to leave the lower bound of the Journal
            if (journalPageIndex == 0) return;

            UpdateJournalSprite(journalPageIndex - 1);
            inspectWindow.SetEntry(Journal.Instance.GetEntryAt(journalPageIndex));
        }

        /// <summary>
        /// Update the Journal sprite
        /// </summary>
        public void UpdateJournalSprite(int indexToCheck)
        {
            // Get the total number of pages
            int numOfPages = Journal.Instance.GetNumOfUnlockedEntries();

            // Set the journal page index and clamp it
            int journalPageIndexToSet = indexToCheck;
            journalPageIndex = Mathf.Clamp(journalPageIndexToSet, 0, numOfPages);

            // Get the journal entry at the page index
            JournalEntry entryToCheck = Journal.Instance.GetEntryAt(journalPageIndex);

            string keyValue = "J_";

            if(numOfPages == 0)
            {
                keyValue += "0_";
            } else if(numOfPages > 0 && numOfPages <= journalPageUpperBound)
            {
                keyValue += "1_";
            } else if(numOfPages > journalPageUpperBound && numOfPages <= journalPageHighestBound)
            {
                keyValue += "2_";
            }

            // Set Hana
            // TODO WAY LATER: Check between Seiji and Hana
            keyValue += "H_";

            if (journalPageIndex == 0)
            {
                // Check if the number of pages is 0
                if(numOfPages == 0)
                {
                    keyValue += $"P{Journal.Instance.GetNumOfNotesCollected()}";
                } else
                {
                    keyValue += "FIRST";
                }
            } else if(journalPageIndex > 0 && journalPageIndex < numOfPages)
            {
                if(!entryToCheck.Completed)
                {
                    keyValue += $"P{entryToCheck.Progress}";
                } else
                {
                    keyValue += "MIDDLE";
                }
            } else if(journalPageIndex == numOfPages)
            {
                keyValue += $"P{entryToCheck.Progress}";
            }

            // Set the journal image
            journalImage.sprite = journalSprites[spriteHashTable[keyValue]];
        }
    }
}
using GoodLuckValley.Events;
using GoodLuckValley.Player.Input;
using GoodLuckValley.UI;
using UnityEngine;

namespace GoodLuckValley.Journal
{
    public class JournalUI : FadePanel
    {
        [SerializeField] private InputReader defaultInputReader;
        [SerializeField] private JournalInputReader journalInputReader;

        [SerializeField] private GameEvent onPause;
        [SerializeField] private GameEvent onSetJournalInputAction;
        [SerializeField] private GameEvent onSetDefaultInputAction;
        [SerializeField] private bool paused;

        [SerializeField] private bool fromPauseMenu;

        private void OnEnable()
        {
            defaultInputReader.OpenJournal += OpenJournalButton;
            journalInputReader.Back += HideJournal;
        }

        private void OnDisable()
        {
            journalInputReader.Back -= HideJournal;
        }

        /// <summary>
        /// Open the Journal UI
        /// </summary>
        public void OpenJournalButton(bool started)
        {
            if (started) return;
            if (!Journal.Instance.Unlocked) return;

            // Pause the game if not paused already
            if (!paused) onPause.Raise(this, null);
            fromPauseMenu = false;

            onSetJournalInputAction.Raise(this, null);

            ShowUI();
        }

        public void OpenJournalMenu()
        {
            if (!Journal.Instance.Unlocked) return;

            fromPauseMenu = true;
            onSetJournalInputAction.Raise(this, null);

            ShowUI();
        }

        public void SetPaused(Component sender, object data)
        {
            // Verify that the correct data has been sent
            if (data is not bool) return;

            // Cast and set data
            paused = (bool)data;
        }

        public void HideJournal(bool started)
        {
            if (started) return;

            if (paused && !fromPauseMenu) onPause.Raise(this, null);

            onSetDefaultInputAction.Raise(this, null);

            HideUI();
        }
    }
}
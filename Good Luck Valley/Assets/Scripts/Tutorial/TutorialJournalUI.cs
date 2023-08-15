using HiveMind.Movement;
using HiveMind.Events;
using HiveMind.Menus;
using HiveMind.NoteJournal;
using UnityEngine;

namespace HiveMind.Tutorial
{
    public class TutorialJournalUI : MonoBehaviour
    {
        #region REFERENCES
        [SerializeField] private DisableScriptableObj disableEvent;
        [SerializeField] private JournalScriptableObj journalEvent;
        private PlayerMovement playerMovement;
        private Tutorial tutorialManager;
        private PauseMenu pauseMenu;
        private Journal journal;
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
            tutorialManager = GameObject.Find("TutorialUI").GetComponent<Tutorial>();
            pauseMenu = GameObject.Find("PauseUI").GetComponent<PauseMenu>();
            journal = GameObject.Find("JournalUI").GetComponent<Journal>();
        }

        public void EndJournalTutorial()
        {
            if (tutorialManager.ShowingThirdJournalUIText)
            {
                if (journalEvent.GetOpenedOnce() && journalEvent.GetJournalOpen())
                {
                    disableEvent.Unlock();
                    tutorialManager.FadeThirdJournalUITutorialText();
                    tutorialManager.ShowingThirdJournalUIText = false;
                }
            }
        }
    }
}


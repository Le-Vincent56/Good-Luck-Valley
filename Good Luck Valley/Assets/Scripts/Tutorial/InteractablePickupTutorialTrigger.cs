using HiveMind.Movement;
using HiveMind.Events;
using HiveMind.Core;
using UnityEngine;

namespace HiveMind.Tutorial
{
    public class InteractablePickupTutorialTrigger : Interactable
    {
        #region REFERENCES
        [SerializeField] private DisableScriptableObj disableEvent;
        [SerializeField] private JournalScriptableObj journalEvent;
        private PlayerMovement playerMovement;
        private Tutorial tutorialManager;
        private TutorialInteractableTrigger interactableTutorialHitBox;
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
            tutorialManager = GameObject.Find("TutorialUI").GetComponent<Tutorial>();
            interactableTutorialHitBox = GameObject.Find("Interactable Tutorial Trigger").GetComponent<TutorialInteractableTrigger>();
        }

        /// <summary>
        /// Remove Tutorial Text on Interaction
        /// </summary>
        public override void Interact()
        {
            // Enable journal
            journalEvent.SetHasJournal(true);
            journalEvent.SetOpenedOnce(false);

            // Lock the player
            disableEvent.Lock();

            // Disable the interactable text
            tutorialManager.ShowingInteractableText = false;
            tutorialManager.ShowingFirstJournalUIText = true;
            interactableTutorialHitBox.Active = false;
            finishedInteracting = true;
        }
    }
}

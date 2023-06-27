using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablePickupTutorialTrigger : Interactable
{
    #region REFERENCES
    [SerializeField] private DisableScriptableObj disableEvent;
    private PlayerMovement playerMovement;
    private Tutorial tutorialManager;
    private TutorialInteractableTrigger interactableTutorialHitBox;
    private Journal journal;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        playerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        tutorialManager = GameObject.Find("TutorialUI").GetComponent<Tutorial>();
        interactableTutorialHitBox = GameObject.Find("Interactable Tutorial Trigger").GetComponent<TutorialInteractableTrigger>();
        journal = GameObject.Find("JournalUI").GetComponent<Journal>();
    }

    /// <summary>
    /// Remove Tutorial Text on Interaction
    /// </summary>
    public override void Interact()
    {
        // Enable journal
        journal.HasJournal = true;
        journal.HasOpened = false;

        // Lock the player
        disableEvent.Lock();

        // Disable the interactable text
        tutorialManager.ShowingInteractableText = false;
        tutorialManager.ShowingFirstJournalUIText = true;
        interactableTutorialHitBox.Active = false;
        finishedInteracting = true;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablePickupTutorialTrigger : Interactable
{
    #region REFERENCES
    private Tutorial tutorialManager;
    private TutorialInteractableTrigger interactableTutorialHitBox;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        tutorialManager = GameObject.Find("TutorialUI").GetComponent<Tutorial>();
        interactableTutorialHitBox = GameObject.Find("Interactable Tutorial Trigger").GetComponent<TutorialInteractableTrigger>();
    }

    /// <summary>
    /// Remove Tutorial Text on Interaction
    /// </summary>
    public override void Interact()
    {
        // Disable the interactable text
        tutorialManager.ShowingInteractableText = false;
        interactableTutorialHitBox.Active = false;
        finishedInteracting = true;
    }
}

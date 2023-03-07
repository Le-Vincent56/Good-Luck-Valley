using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractablePickupTutorialTrigger : Interactable
{
    private Tutorial tutorialManager;
    private TutorialInteractableTrigger interactableTutorialHitBox;

    // Start is called before the first frame update
    void Start()
    {
        tutorialManager = GameObject.Find("TutorialUI").GetComponent<Tutorial>();
        interactableTutorialHitBox = GameObject.Find("Interactable Tutorial Trigger").GetComponent<TutorialInteractableTrigger>();
    }

    public override void Interact()
    {
        tutorialManager.ShowingInteractableText = false;
        interactableTutorialHitBox.active = false;
        finishedInteracting = true;
    }
}

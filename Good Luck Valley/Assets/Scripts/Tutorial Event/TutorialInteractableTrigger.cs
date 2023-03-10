using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInteractableTrigger : MonoBehaviour
{
    private Tutorial tutorialManager;
    public bool active = true;

    // Start is called before the first frame update
    void Start()
    {
        tutorialManager = GameObject.Find("TutorialUI").GetComponent<Tutorial>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player") && active)
        {
            tutorialManager.ShowingInteractableText = true;
        }
    }
}

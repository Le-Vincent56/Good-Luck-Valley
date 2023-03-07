using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMovementTrigger : MonoBehaviour
{
    private Tutorial tutorialManager;

    // Start is called before the first frame update
    void Start()
    {
        tutorialManager = GameObject.Find("TutorialUI").GetComponent<Tutorial>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag.Equals("Player"))
        {
            tutorialManager.ShowingMovementText = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Equals("Player"))
        {
            tutorialManager.ShowingMovementText = false;
        }
    }
}

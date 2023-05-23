using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialLotusTrigger : MonoBehaviour
{
    private Tutorial tutorialManager;

    // Start is called before the first frame update
    void Start()
    {
        tutorialManager = GameObject.Find("TutorialUI").GetComponent<Tutorial>();
    }

    /// <summary>
    /// Show the Lotus Tutorial text
    /// </summary>
    /// <param name="collision">The collider causing the trigger</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the collider is the Player, show the Lotus tutorial text
        if (collision.gameObject.tag.Equals("Player"))
        {
            tutorialManager.ShowingLotusText = true;
        }
    }
}

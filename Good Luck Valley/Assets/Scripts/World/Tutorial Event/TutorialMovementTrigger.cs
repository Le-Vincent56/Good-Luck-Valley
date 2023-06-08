using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialMovementTrigger : MonoBehaviour
{
    #region REFERENCES
    private Tutorial tutorialManager;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        tutorialManager = GameObject.Find("TutorialUI").GetComponent<Tutorial>();
    }

    /// <summary>
    /// Show Tutorial Text when entering the Collision area
    /// </summary>
    /// <param name="collision">The collider causing the Trigger</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the Player is the colliding object, show the tutorial movement text
        if(collision.gameObject.tag.Equals("Player"))
        {
            Debug.Log("Movement");
            tutorialManager.ShowingMovementText = true;
        }
    }

    /// <summary>
    /// Hide Tutorial Text when exiting the Collision area
    /// </summary>
    /// <param name="collision">The collider causing the Trigger</param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        // If the Player is the colliding object, hide the tutorial movement text
        if (collision.gameObject.tag.Equals("Player"))
        {
            tutorialManager.ShowingMovementText = false;
        }
    }
}

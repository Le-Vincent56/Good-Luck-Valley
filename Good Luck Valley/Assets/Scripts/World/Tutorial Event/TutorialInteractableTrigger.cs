using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInteractableTrigger : MonoBehaviour
{
    #region REFERENCES
    private Tutorial tutorialManager;
    #endregion

    #region FIELDS
    private bool active = true;
    #endregion

    #region PROPERTIES
    public bool Active { get { return active; } set { active = value; } }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        tutorialManager = GameObject.Find("TutorialUI").GetComponent<Tutorial>();
    }

    /// <summary>
    /// Show Interactable text
    /// </summary>
    /// <param name="collision">The collider causing the trigger</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the collider is the Player and the InteractableText is active, show 
        // the interactable tutorial text
        if (collision.gameObject.tag.Equals("Player") && active)
        {
            tutorialManager.ShowingInteractableText = true;
        }
    }
}

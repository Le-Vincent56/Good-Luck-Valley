using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutoriaMushroomInteract : MonoBehaviour
{
    #region REFERENCES
    private Tutorial tutorialManager;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        tutorialManager = GameObject.Find("TutorialUI").GetComponent<Tutorial>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Show Tutorial Text when entering the Collision area
    /// </summary>
    /// <param name="collision">The collider causing the Trigger</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the Player is the colliding object, show the tutorial movement text
        if (collision.gameObject.tag.Equals("Player"))
        {
            tutorialManager.ShowingMushroomInteractText = true;
        }
    }


}

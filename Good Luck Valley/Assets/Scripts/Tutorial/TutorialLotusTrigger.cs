using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HiveMind.Tutorial
{
    public class TutorialLotusTrigger : MonoBehaviour
    {
        private Tutorial tutorialManager;

        // Start is called before the first frame update
        void Start()
        {
            // CHanged this to shroomevent for rockstar build
            tutorialManager = GameObject.Find("ShroomEvent").GetComponent<Tutorial>();
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
                tutorialManager.ShowLotusTutorialText();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            // If the collider is the Player and the InteractableText is active, show 
            // the interactable tutorial text
            if (collision.gameObject.tag.Equals("Player"))
            {
                tutorialManager.FadeLotusTutorialText();
            }
        }
    }
}

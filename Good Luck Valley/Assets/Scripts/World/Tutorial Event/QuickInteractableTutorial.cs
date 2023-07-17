using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class QuickInteractableTutorial : MonoBehaviour
{
    private Image interactPanel;
    private Text interactText;

    // Start is called before the first frame update
    void Start()
    {
        // CHanged this to shroomevent for rockstar build
        interactPanel = GameObject.Find("Mushroom Interact Tutorial Panel").GetComponent<Image>();
        interactText = GameObject.Find("Mushroom Interact Tutorial Text").GetComponent<Text>();
        interactPanel.color = new Color(interactPanel.color.r, interactPanel.color.g, interactPanel.color.b, 0);
        interactText.color = new Color(interactText.color.r, interactText.color.g, interactText.color.b, 0);
    }

    /// <summary>
    /// Show the Lotus Tutorial text
    /// </summary>
    /// <param name="collision">The collider causing the trigger</param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // If the collider is the Player, show the Lotus tutorial text
        if (collision.tag == "Player" && gameObject.activeSelf)
        {
            StartCoroutine(ShowInteract());
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // If the collider is the Player and the InteractableText is active, show 
        // the interactable tutorial text
        if (collision.tag == "Player" && gameObject.activeSelf)
        {
            StartCoroutine(HideInteract());
        }
    }

    private IEnumerator ShowInteract()
    {
        while (interactPanel.color.a < 1 && interactText.color.a < 1)
        {
            string tutorialText = "Press E";

            // Set text
            interactText.text = tutorialText;

            // Fade in the text using deltaTime and alpha values
            if (interactPanel.color.a < 1 && interactText.color.a < 1)
            {
                interactPanel.color = new Color(interactPanel.color.r, interactPanel.color.g, interactPanel.color.b, interactPanel.color.a + 0.01f);
                interactText.color = new Color(interactText.color.r, interactText.color.g, interactText.color.b, interactText.color.a + 0.01f);
            }
            yield return null;
        }
    }

    private IEnumerator HideInteract()
    {
        while (interactPanel.color.a > 0 && interactText.color.a > 0)
        {
            // Fade out the text using deltaTime and alpha values
            if (interactPanel.color.a > 0 && interactText.color.a > 0)
            {
                interactPanel.color = new Color(interactPanel.color.r, interactPanel.color.g, interactPanel.color.b, interactPanel.color.a - 0.02f);
                interactText.color = new Color(interactText.color.r, interactText.color.g, interactText.color.b, interactText.color.a - 0.02f);
            }
            yield return null;
        }
    }
}

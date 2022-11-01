using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractManager : MonoBehaviour
{
    public List<Interactable> interactables;
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        // Retrieve variables
        interactables.AddRange(FindObjectsOfType<Interactable>());
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // Iterate through each interactable
        foreach(Interactable interactable in interactables)
        {
            // Check if the player is in range to interact
            if (interactable.GetComponent<BoxCollider2D>().IsTouching(player.GetComponent<BoxCollider2D>()))
            {
                interactable.inRange = true;
            } else
            {
                interactable.inRange = false;
            }
        }
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        // Interate through interactables
        foreach(Interactable interactable in interactables)
        {
            // CHeck if any are in range
            if (interactable.inRange)
            {
                // If so, and the interact button is pressed, trigger the interaction
                if (context.started)
                {
                    interactable.controlTriggered = true;
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractManager : MonoBehaviour
{
    public List<Interactable> interactables;
    public List<Interactable> destroyedInteractables;
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

        // Remove Interactables
        CheckRemovedInteractables();
        RemoveInteractables();
    }

    public void CheckRemovedInteractables()
    {
        // Iterate through each interactable
        foreach(Interactable interactable in interactables)
        {
            // If the interactable is supposed to be removed, add it
            // to the destroyed interactables list
            if(interactable.remove)
            {
                destroyedInteractables.Add(interactable);
            }
        }
    }

    public void RemoveInteractables()
    {
        // Check the list of destroyed interactables
        if (destroyedInteractables.Count > 0)
        {
            // Compare the list of destroyed interactables with the total interactables
            for (int i = 0; i < destroyedInteractables.Count; i++)
            {
                for (int j = 0; j < interactables.Count; j++)
                {
                    // If any of the destroyed interactables are equal to any of the interactables
                    // in the interactables List, remove them from the List
                    if (destroyedInteractables[i].Equals(interactables[j]))
                    {
                        Destroy(interactables[j].gameObject);
                        interactables.Remove(destroyedInteractables[i]);
                    }
                }
            }

            // Clear the destroyed interactables List
            destroyedInteractables.Clear();
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

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
        interactables.AddRange(FindObjectsOfType<Interactable>());
        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        foreach(Interactable interactable in interactables)
        {
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
        foreach(Interactable interactable in interactables)
        {
            if (interactable.inRange)
            {
                if (context.started)
                {
                    interactable.controlTriggered = true;
                }
            }
        }
    }
}

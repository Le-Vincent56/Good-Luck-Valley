using HiveMind.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HiveMind.Interactables
{
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
            foreach (Interactable interactable in interactables)
            {
                // Check if the player is in range to interact
                if (!interactable.OverridesRangeDetection)
                {
                    if (interactable.GetComponent<BoxCollider2D>().IsTouching(player.GetComponent<BoxCollider2D>()))
                    {
                        interactable.InRange = true;
                    }
                    else
                    {
                        interactable.InRange = false;
                    }
                }
            }

            // Remove Interactables
            foreach (Interactable interactable in interactables)
            {
                if (interactable.Remove)
                {
                    interactable.gameObject.SetActive(false);
                    interactable.gameObject.GetComponent<Renderer>().enabled = false;
                }
            }
        }

        /// <summary>
        /// Interact using the control scheme
        /// </summary>
        /// <param name="context">The context of the Controller</param>
        public void OnInteract(InputAction.CallbackContext context)
        {
            // Interate through interactables
            foreach (Interactable interactable in interactables)
            {
                // CHeck if any are in range
                if (interactable.InRange)
                {
                    // If so, and the interact button is pressed, trigger the interaction
                    if (context.started)
                    {
                        interactable.ControlTriggered = true;
                    }
                }
            }
        }
    }
}

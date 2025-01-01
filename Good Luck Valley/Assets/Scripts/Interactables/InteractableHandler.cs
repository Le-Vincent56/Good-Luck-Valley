using GoodLuckValley.Architecture.Optionals;
using GoodLuckValley.Input;
using GoodLuckValley.Interactables.Fireflies;
using GoodLuckValley.Player.Movement;
using UnityEngine;

namespace GoodLuckValley.Interactables
{
    public class InteractableHandler : MonoBehaviour
    {
        [SerializeField] private GameInputReader inputReader;
        private Optional<Interactable> currentInteractable;
        private PlayerController controller;
        private FireflyHandler fireflyHandler;

        [SerializeField] private string interactableName;

        public PlayerController Controller { get => controller; }
        public FireflyHandler FireflyHandler { get => fireflyHandler; }

        private void Awake()
        {
            // Get components
            controller = GetComponent<PlayerController>();
            fireflyHandler = GetComponent<FireflyHandler>();
        }

        protected virtual void OnEnable()
        {
            inputReader.Interact += Interact;
        }

        protected virtual void OnDisable()
        {
            inputReader.Interact -= Interact;
        }

        /// <summary>
        /// Interact with the current Interactable
        /// </summary>
        private void Interact(bool started)
        {
            // Exit case - if the input is being canceled instead of started
            if (!started) return;

            // Handle the Interactable, if it exists
            currentInteractable.Match(
                onValue: interactable => 
                {
                    // If there's a value, interact with it
                    interactable.Interact();
                    return 0;
                },
                onNoValue: () => { return 0; }
            );
        }

        /// <summary>
        /// Set an Interactable to be handled
        /// </summary>
        public void SetInteractable(Optional<Interactable> interactable)
        {
            // Set the current Interactable
            currentInteractable = interactable;

            // Set a name, if it exists
            currentInteractable.Match(
                onValue: interactable =>
                {
                    interactableName = interactable.gameObject.name;
                    return 0;
                },
                onNoValue: () => 
                {
                    interactableName = string.Empty;
                    return 0; 
                }
            );
        }
    }
}

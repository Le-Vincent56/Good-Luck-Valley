using GoodLuckValley.Architecture.StateMachine;
using UnityEngine;

namespace GoodLuckValley.UI.MainMenu.StartMenu
{
    public class DeleteOverlay : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private StartMenuController controller;
        [SerializeField] private ConfirmationMenu confirmationMenu;
        [SerializeField] private GameObject deleteOverlayObject;
        [SerializeField] private Animator animator;

        [Header("Fields")]
        [SerializeField] private int state;

        private StateMachine stateMachine;

        public int IDLE { get => 0; }
        public int POPUP { get => 1; }
        public int DELETING { get => 2; }

        private void Awake()
        {
            // Get components
            controller = GetComponent<StartMenuController>();

            // Set up the State Machine
            SetupStateMachine();
        }

        private void Update()
        {
            // Update the State Machine
            stateMachine.Update();
        }

        /// <summary>
        /// Set up the State Machine
        /// </summary>
        private void SetupStateMachine()
        {
            // Initialize the State Machine
            stateMachine = new StateMachine();

            // Create states

            // Define state transitions

            // Set the initial state
        }

        /// <summary>
        /// Set the delete overlay state
        /// </summary>
        /// <param name="state">The state to set to</param>
        public void SetState(int state) => this.state = state;

        /// <summary>
        /// Get the delete overlay state
        /// </summary>
        /// <returns></returns>
        public int GetState() => state;

        /// <summary>
        /// Delete the selected slot's data
        /// </summary>
        public void DeleteData() => controller.DeleteData(controller.SelectedSlot);
    }
}

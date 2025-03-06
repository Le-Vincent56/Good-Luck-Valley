using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.UI.Menus.Start.States;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace GoodLuckValley.UI.Menus.Start
{
    public class DeleteOverlay : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private StartMenuController controller;
        [SerializeField] private ConfirmationMenu confirmationMenu;
        [SerializeField] private Image contrastOverlay;
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Animator animator;

        [Header("Fields")]
        [SerializeField] private int state;

        private StateMachine stateMachine;

        public int IDLE { get => 0; }
        public int POPUP { get => 1; }
        public int DELETING { get => 2; }

        public int State { get => state; }

        private void Awake()
        {
            // Get components
            canvasGroup = GetComponent<CanvasGroup>();
            animator = GetComponentInChildren<Animator>();

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
            DeleteIdleState idleState = new DeleteIdleState(this, animator, canvasGroup, contrastOverlay);
            DeletePopupState popupState = new DeletePopupState(this, animator, canvasGroup, confirmationMenu, contrastOverlay);
            DeleteConfirmState confirmState = new DeleteConfirmState(this, animator, canvasGroup, animator.GetComponent<Image>(), contrastOverlay);

            // Define state transitions
            stateMachine.At(idleState, popupState, new FuncPredicate(() => state == POPUP));
            stateMachine.At(popupState, idleState, new FuncPredicate(() => state == IDLE));
            stateMachine.At(popupState, confirmState, new FuncPredicate(() => state == DELETING));
            stateMachine.At(confirmState, idleState, new FuncPredicate(() => state == IDLE));

            // Set the initial state
            stateMachine.SetState(idleState);
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

        /// <summary>
        /// Set the Save Slot data
        /// </summary>
        public void SetSlotData() => controller.SetSlotData();

        /// <summary>
        /// Return to the Slot Deleter
        /// </summary>
        public void ReturnToSlotDeleter() => controller.SetAndEnableSlots(false);
    }
}

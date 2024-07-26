using GoodLuckValley.Patterns.StateMachine;
using GoodLuckValley.UI.Menus;
using GoodLuckValley.UI.TitleScreen.Start.States;
using UnityEngine;
using UnityEngine.UI;

namespace GoodLuckValley.UI.TitleScreen.Start
{
    public class DeleteOverlayController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private StartMenuController controller;
        [SerializeField] private GameObject deleteOverlayObject;
        [SerializeField] private GameObject deleteAnimatorObject;
        [SerializeField] private ConfirmationPopupMenu popupMenu;
        [SerializeField] private Animator animator;
        [SerializeField] private MenuCursor deleteCursor;

        [Header("Fields")]
        [SerializeField] private int state;

        private StateMachine stateMachine;

        public int IDLE { get => 0; }
        public int CONFIRMATION { get => 1; }
        public int DELETING { get => 2; }

        private void Awake()
        {
            // Get components
            controller = GetComponent<StartMenuController>();
            animator = deleteAnimatorObject.GetComponentInChildren<Animator>();

            // Instantiate the UI lists for the popup
            popupMenu.InstantiateUILists();
            popupMenu.MakeElementsInvisible();

            // Initialize the state machine
            stateMachine = new StateMachine();

            // Construct states
            IdleDeleteState idleState = new IdleDeleteState(this, animator, deleteOverlayObject, deleteAnimatorObject);
            DeletePopupState popupState = new DeletePopupState(this, animator, deleteOverlayObject, deleteAnimatorObject, popupMenu, deleteCursor);
            DeletingDeleteState deleteState = new DeletingDeleteState(this, animator, deleteOverlayObject, deleteAnimatorObject);

            // Set state transitions
            At(idleState, popupState, new FuncPredicate(() => state == CONFIRMATION));
            At(popupState, idleState, new FuncPredicate(() => state == IDLE));
            At(popupState, deleteState, new FuncPredicate(() => state == DELETING));
            At(deleteState, idleState, new FuncPredicate(() => state == IDLE));

            // Set initial state
            stateMachine.SetState(idleState);

            // Make the images from the objects invisible
            Image deleteOverlay = deleteOverlayObject.GetComponent<Image>();
            Image animatedImage = deleteAnimatorObject.GetComponent<Image>();

            Color backgroundInvis = deleteOverlay.color;
            Color animatedInvis = animatedImage.color;
            backgroundInvis.a = 0f;
            animatedInvis.a = 0f;
            deleteOverlay.color = backgroundInvis;
            animatedImage.color = animatedInvis;
        }

        private void Update()
        {
            // Update the state machine
            stateMachine.Update();
        }

        /// <summary>
        /// Add a transition from one State to another given a certain condition
        /// </summary>
        /// <param name="from">The State to define the transition from</param>
        /// <param name="to">The State to define the transition to</param>
        /// <param name="condition">The condition of the Transition</param>
        private void At(IState from, IState to, IPredicate condition) => stateMachine.AddTransition(from, to, condition);

        /// <summary>
        /// Set the delete overlay state
        /// </summary>
        /// <param name="state">The state to set to</param>
        public void SetState(int state) => this.state = state;

        /// <summary>
        /// Delete the selected slot's data
        /// </summary>
        public void DeleteData() => controller.DeleteData(controller.SelectedSlot);
    }
}
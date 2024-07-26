using UnityEngine.InputSystem;
using UnityEngine;
using GoodLuckValley.Patterns.StateMachine;
using GoodLuckValley.UI.TitleScreen.Settings.Controls.States;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using GoodLuckValley.Player.Input;

namespace GoodLuckValley.UI.TitleScreen.Settings.Controls
{
    public class ControlsSettingController : SettingsController
    {
        [Header("References")]
        [SerializeField] private InputKeyDictionary keysDict;
        [SerializeField] private MenuInputReader menuInputReader;
        [SerializeField] private InputActionAsset inputActionAsset;
        [SerializeField] private GameObject rebindPopup;

        [Header("Fields")]
        [SerializeField] private float fadeDuration = 0.5f;
        [SerializeField] private bool binding;
        [SerializeField] private List<RebindingButton> rebindingButtons = new List<RebindingButton>();

        private StateMachine stateMachine;
        private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

        protected override void Awake()
        {
            base.Awake();

            // Create state machine
            stateMachine = new StateMachine();

            // Construct states
            IdleControlsState idleState = new IdleControlsState(this, rebindPopup);
            BindingControlsState bindingState = new BindingControlsState(this, rebindPopup);

            // Set state transitions
            At(idleState, bindingState, new FuncPredicate(() => binding));
            At(bindingState, idleState, new FuncPredicate(() => !binding));

            // Set the initial state
            stateMachine.SetState(idleState);

            // Initialize each rebinding button
            foreach (RebindingButton rebindingButton in rebindingButtons)
            {
                rebindingButton.Init(this);
            }
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
        /// Start rebinding
        /// </summary>
        /// <param name="actionName"></param>
        /// <param name="bindingIndex"></param>
        public void StartRebinding(string actionName, int bindingIndex, RebindingButton rebindingButton)
        {
            // Get the action name
            InputAction action = inputActionAsset.FindAction(actionName);

            // Exit case - if the action doesn't exists
            if (action == null) return;

            // Prevent navigation events
            EventSystem.current.sendNavigationEvents = false;

            // Disable menu input
            menuInputReader.Disable();

            // Set binding to true
            binding = true;

            rebindingOperation = action.PerformInteractiveRebinding(bindingIndex)
                .WithCancelingThrough("")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation => RebindComplete(action, bindingIndex, rebindingButton))
                .Start();
        }

        /// <summary>
        /// Cancel rebinding
        /// </summary>
        private void RebindCancel() => ResumeMenuOperations();

        /// <summary>
        /// Finalize rebinding
        /// </summary>
        private void RebindComplete(InputAction action, int bindingIndex, RebindingButton rebindingButton)
        {
            // Set the new binding button
            rebindingButton.SetImage(keysDict.GetKey(action.bindings[bindingIndex].ToDisplayString()));

            // Resume menu operations
            ResumeMenuOperations();
        }

        /// <summary>
        /// Resume normal menu operations
        /// </summary>
        private void ResumeMenuOperations()
        {
            // Set binding to false
            binding = false;

            // Dispose of the rebinding operation
            rebindingOperation.Dispose();

            // Allow navigation events
            EventSystem.current.sendNavigationEvents = true;

            // Enable menu input
            menuInputReader.Enable();
        }

        public void BackToSettings() => controller.SetState(controller.SETTINGS);
        public void ResetSettings() { }
    }
}
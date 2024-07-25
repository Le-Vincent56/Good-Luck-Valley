using UnityEngine.InputSystem;
using UnityEngine;
using GoodLuckValley.Patterns.StateMachine;
using GoodLuckValley.UI.TitleScreen.Settings.Controls.States;
using System.Collections.Generic;

namespace GoodLuckValley.UI.TitleScreen.Settings.Controls
{
    public class ControlsSettingController : SettingsController
    {
        [Header("References")]
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
        public void StartRebinding(string actionName, int bindingIndex)
        {
            InputAction action = inputActionAsset.FindAction(actionName);

            if (action == null) return;

            // Set binding to true
            binding = true;

            rebindingOperation = action.PerformInteractiveRebinding(bindingIndex)
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation => RebindComplete())
                .Start();
        }

        /// <summary>
        /// Finalize rebinding
        /// </summary>
        private void RebindComplete()
        {
            // Set binding to false
            binding = false;

            // Dispose of the rebinding operation
            rebindingOperation.Dispose();
        }

        public void BackToSettings() => controller.SetState(controller.SETTINGS);
        public void ResetSettings() { }
    }
}
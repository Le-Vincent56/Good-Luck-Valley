using GoodLuckValley.Architecture.StateMachine;
using GoodLuckValley.Input;
using GoodLuckValley.UI.MainMenu.Controls.States;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace GoodLuckValley.UI.MainMenu.Controls
{
    public class ControlsMenuController : MonoBehaviour
    {
        //[Header("Wwise Events")]
        //[SerializeField] private AK.Wwise.Event playButtonGeneral;
        //[SerializeField] private AK.Wwise.Event playButtonReset;

        [Header("References")]
        [SerializeField] private InputKeyDictionary keysDict;
        [SerializeField] private UIInputReader inputReader;
        [SerializeField] private InputActionAsset inputActionAsset;
        [SerializeField] private Text duplicatesText;
        [SerializeField] private Animator[] animators;
        [SerializeField] private RebindButton[] rebindingButtons;

        [Header("Fields")]
        [SerializeField] private bool binding;
        [SerializeField] private int currentRebindingButton;

        private StateMachine stateMachine;
        private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

        public int CurrentRebindingButton { get => currentRebindingButton; }

        private void Awake()
        {
            // Get components
            animators = GetComponentsInChildren<Animator>();
            rebindingButtons = GetComponentsInChildren<RebindButton>();

            // Iterate through each Rebind Button
            foreach(RebindButton rebindingButton in rebindingButtons)
            {
                // Initialize the Rebind Button
                rebindingButton.Initialize(this, inputActionAsset, keysDict);
            }

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
            // Create state machine
            stateMachine = new StateMachine();

            // Construct states
            ControlsIdleState idleState = new ControlsIdleState(this);
            ControlsRebindState bindingState = new ControlsRebindState(this, animators);

            // Set state transitions
            stateMachine.At(idleState, bindingState, new FuncPredicate(() => binding));
            stateMachine.At(bindingState, idleState, new FuncPredicate(() => !binding));

            // Set the initial state
            stateMachine.SetState(idleState);
        }

        /// <summary>
        /// Start rebinding
        /// </summary>
        public void StartRebinding(string actionName, int bindingIndex, RebindButton rebindingButton)
        {
            // Get the action name
            InputAction action = inputActionAsset.FindAction(actionName);

            // Exit case - if the action doesn't exists
            if (action == null) return;

            // Get the binding button
            currentRebindingButton = Array.IndexOf(rebindingButtons, rebindingButton);

            // Prevent navigation events
            EventSystem.current.sendNavigationEvents = false;

            // Disable menu input
            inputReader.Disable();

            // Set binding to true
            binding = true;

            // Disable the action before rebinding
            action.Disable();

            //// Play the general button sound
            //playButtonGeneral.Post(gameObject);

            rebindingOperation = action.PerformInteractiveRebinding(bindingIndex)
                .WithCancelingThrough("")
                .OnMatchWaitForAnother(0.1f)
                .OnComplete(operation =>
                {
                    // Re-enable actions
                    action.Enable();

                    // Set the new binding button
                    rebindingButton.SetImage(keysDict.GetKey(action.bindings[bindingIndex].ToDisplayString()));

                    if (CheckDuplicateBindings(action, bindingIndex))
                    {
                        // Remove any binding overrides
                        action.RemoveBindingOverride(bindingIndex);

                        // Complete a failed rebinding
                        RebindFailedComplete(rebindingButton);
                        return;
                    }

                    // Complete a successful rebinding
                    RebindSuccessfulComplete(rebindingButton);
                })
                .Start();
        }

        /// <summary>
        /// Check for duplicate bindings
        /// </summary>
        private bool CheckDuplicateBindings(InputAction action, int bindingIndex)
        {
            // Store the new binding
            InputBinding newBinding = action.bindings[bindingIndex];

            // Loop through each binding
            foreach (InputBinding binding in action.actionMap.bindings)
            {
                // If the action is the same as the one being rebinded, continue
                if (binding.action == newBinding.action)
                    continue;

                // If the action is Moving Up and set to W (only used for dev tools), ignore FOR NOW
                if (binding.name == "up" && binding.effectivePath == "<Keyboard>/w")
                    continue;

                // Check if the control at the binding is the same as the new one
                if (binding.effectivePath == newBinding.effectivePath)
                {
                    // If so, return true
                    return true;
                }
            }

            // Check for duplicate composite bindings
            if (action.bindings[bindingIndex].isPartOfComposite)
            {
                // Start the composite index at 1 (0 is the "head" or the describer)
                int compositeIndex = bindingIndex;
                while (compositeIndex > 0 && action.bindings[compositeIndex].isPartOfComposite)
                {
                    compositeIndex--;
                }

                // Loop through each control that's part of the same composite
                for (int i = compositeIndex + 1; i < action.bindings.Count && action.bindings[i].isPartOfComposite; i++)
                {
                    // If the action is the same as the one being rebinded, continue
                    if (action.actionMap.bindings[i].name == newBinding.name)
                        continue;

                    // If the action is Moving Up and set to W (only used for dev tools), ignore FOR NOW
                    if (action.actionMap.bindings[i].name == "up" && action.actionMap.bindings[i].effectivePath == "<Keyboard>/w")
                        continue;

                    // Return true if the bindings are the same
                    if (action.actionMap.bindings[i].effectivePath == newBinding.effectivePath)
                        return true;
                }
            }

            // If made it out, then there are no duplicates
            return false;
        }

        /// <summary>
        /// Finalize a successful rebinding
        /// </summary>
        private void RebindSuccessfulComplete(RebindButton rebindingButton)
        {
            rebindingButton.Rebinded = false;

            // End the rebinding
            EndRebinding(rebindingButton);

            // Set a valid rebind
            rebindingButton.SetValidRebind(true);

            // Update whether or not the rebinding button has been rebinded
            // (or has been attempted to be rebinded)
            rebindingButton.UpdateRebinded();
        }

        /// <summary>
        /// Finalize a failed rebinding
        /// </summary>
        private void RebindFailedComplete(RebindButton rebindingButton)
        {
            // Update whether or not the rebinding button has been rebinded
            rebindingButton.Rebinded = true;

            // End the rebinding
            EndRebinding(rebindingButton);

            // Set an invalid rebind
            rebindingButton.SetValidRebind(false);
        }

        /// <summary>
        /// End the rebinding process
        /// </summary>
        private void EndRebinding(RebindButton rebindingButton)
        {
            // Set binding to false
            binding = false;

            // Dispose of the rebinding operation
            rebindingOperation.Dispose();

            // Allow navigation events
            EventSystem.current.sendNavigationEvents = true;

            // Enable menu input
            inputReader.Enable();
        }

        /// <summary>
        /// Activate the current rebinding button's animator
        /// </summary>
        public void ActivateRebindingButtonAnimator() => rebindingButtons[currentRebindingButton].EnableAnimator();

        /// <summary>
        /// De-activate the current rebinding button's animator
        /// </summary>
        public void DeactivateRebindingButtonAnimator() => rebindingButtons[currentRebindingButton].DisableAnimator();

        /// <summary>
        /// Reset the key bindings
        /// </summary>
        public void ResetSettings()
        {
            // Loop through each map in the action assets
            foreach (InputActionMap map in inputActionAsset.actionMaps)
            {
                // Remove all bindings
                map.RemoveAllBindingOverrides();
            }

            // Set the default settings for each rebinding button
            foreach (RebindButton rebindingButton in rebindingButtons)
            {
                rebindingButton.SetDefault(inputActionAsset, keysDict);
            }

            // Play the reset button sound
            //playButtonReset.Post(gameObject);
        }
    }
}

using UnityEngine.InputSystem;
using UnityEngine;
using GoodLuckValley.Patterns.StateMachine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using GoodLuckValley.Player.Input;
using UnityEngine.UI;
using System.Collections;
using GoodLuckValley.UI.Settings.Controls.States;

namespace GoodLuckValley.UI.Settings.Controls
{
    public class GameControlsSettingController : MonoBehaviour
    {
        [Header("Wwise Events")]
        [SerializeField] private AK.Wwise.Event playButtonReset;
        [SerializeField] private AK.Wwise.Event playButtonGeneral;

        [Header("References")]
        [SerializeField] private GameSettingsMenu controller;
        [SerializeField] private InputKeyDictionary keysDict;
        [SerializeField] private MenuInputReader menuInputReader;
        [SerializeField] private InputActionAsset inputActionAsset;
        [SerializeField] private ControlsSaveHandler controlSaveHandler;
        [SerializeField] private Text duplicatesText;

        [Header("Fields")]
        private const int stateNum = 6;
        [SerializeField] private float fadeDuration = 0.01f;
        [SerializeField] private bool binding;
        [SerializeField] private int currentBindingButton;
        [SerializeField] private List<Animator> animators = new List<Animator>();
        [SerializeField] private List<GameRebindingButton> rebindingButtons = new List<GameRebindingButton>();
        private Coroutine duplicateFadeCoroutine;

        private StateMachine stateMachine;
        private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

        public int CurrentRebindingButton { get => currentBindingButton; }
        public string LastRebindingAction { get; private set; }

        private void Awake()
        {
            // Initialize
            Init(this, null);
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
        public void StartRebinding(string actionName, int bindingIndex, GameRebindingButton rebindingButton)
        {
            // Get the action name
            InputAction action = inputActionAsset.FindAction(actionName);

            // Exit case - if the action doesn't exists
            if (action == null) return;

            // Get the binding button
            currentBindingButton = rebindingButtons.IndexOf(rebindingButton);

            // Prevent navigation events
            EventSystem.current.sendNavigationEvents = false;

            // Disable menu input
            menuInputReader.Disable();

            // Set binding to true
            binding = true;

            // Disable the action before rebinding
            action.Disable();

            // Play the general button sound
            playButtonGeneral.Post(gameObject);

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
        /// <param name="action"></param>
        /// <param name="bindingIndex"></param>
        /// <returns></returns>
        private bool CheckDuplicateBindings(InputAction action, int bindingIndex)
        {
            // Store the new binding
            InputBinding newBinding = action.bindings[bindingIndex];

            // Loop through each binding
            foreach(InputBinding binding in action.actionMap.bindings)
            {
                // If the action is the same as the one being rebinded, continue
                if(binding.action == newBinding.action)
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
            if(action.bindings[bindingIndex].isPartOfComposite)
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
        private void RebindSuccessfulComplete(GameRebindingButton rebindingButton)
        {
            // Update whether or not the rebinding button has been rebinded
            rebindingButton.SetRebinded(true);

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
        /// <param name="action"></param>
        /// <param name="bindingIndex"></param>
        /// <param name="rebindingButton"></param>
        private void RebindFailedComplete(GameRebindingButton rebindingButton)
        {
            // Update whether or not the rebinding button has been rebinded
            rebindingButton.SetRebinded(false);

            // End the rebinding
            EndRebinding(rebindingButton);

            // Set an invalid rebind
            rebindingButton.SetValidRebind(false);
        }

        /// <summary>
        /// End the rebinding process
        /// </summary>
        private void EndRebinding(GameRebindingButton rebindingButton)
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

        private IEnumerator FadeInDuplicatesText(float targetAlpha)
        {
            // If the game object is disabled, enable it
            if(!duplicatesText.gameObject.activeSelf) duplicatesText.gameObject.SetActive(true);

            float elapsedTime = 0f;
            Color color = duplicatesText.color;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                color.a = Mathf.Clamp(elapsedTime / fadeDuration, 0f, targetAlpha);
                duplicatesText.color = color;
                yield return null;
            }
        }

        private IEnumerator FadeOutDuplicatesText()
        {
            float elapsedTime = 0f;
            Color color = duplicatesText.color;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                color.a = Mathf.Clamp01(1.0f - (elapsedTime / fadeDuration));
                duplicatesText.color = color;
                yield return null;
            }

            color.a = 1f;
            duplicatesText.color = color;

            // If the game object is enabled, disable it
            if (duplicatesText.gameObject.activeSelf) duplicatesText.gameObject.SetActive(false);
        }

        /// <summary>
        /// Get the current rebinding button
        /// </summary>
        /// <returns></returns>
        public GameRebindingButton GetCurrentRebindingButton() => rebindingButtons[CurrentRebindingButton];

        /// <summary>
        /// Activate the current rebinding button's animator
        /// </summary>
        public void ActivateRebindingButtonAnimator() => GetCurrentRebindingButton().EnableAnimator();

        /// <summary>
        /// De-activate the current rebinding button's animator
        /// </summary>
        public void DeactivateRebindingButtonAnimator() => GetCurrentRebindingButton().DisableAnimator();

        private bool CheckValidity()
        {
            // Loop through each rebinding button
            foreach(GameRebindingButton rebindingButton in rebindingButtons)
            {
                // If the rebinding button has a valid rebind, return false
                if (!rebindingButton.GetValidRebind())
                    return false;
            }

            // If made it out of the loop, return true
            return true;
        }

        /// <summary>
        /// Go back to the settings menu
        /// </summary>
        public void BackToSettings()
        {
            // Check the validity of the rebindingv buttons
            if(CheckValidity())
            {
                // Nullify any fade coroutines
                if (duplicateFadeCoroutine != null)
                {
                    StopCoroutine(duplicateFadeCoroutine);
                    duplicateFadeCoroutine = null;
                }

                // Start the fade out coroutine
                duplicateFadeCoroutine = StartCoroutine(FadeOutDuplicatesText());

                // Set all rebinded buttons to false
                foreach (GameRebindingButton rebindingButton in rebindingButtons)
                {
                    rebindingButton.UpdateRebinded();
                }

                // Save player keybindings
                controlSaveHandler.SaveData();

                // Set the settings state
                controller.SetState(controller.MAIN);
            } else
            {
                // Nullify any fade coroutines
                if (duplicateFadeCoroutine != null)
                {
                    StopCoroutine(duplicateFadeCoroutine);
                    duplicateFadeCoroutine = null;
                }

                // Start the fade in coroutine
                duplicateFadeCoroutine = StartCoroutine(FadeInDuplicatesText(1f));
            }
        }

        public void BackInput(Component sender, object data)
        {
            // Verify that the correct data was sent
            if (data is not int) return;

            // Cast and compare data
            if((int)data == stateNum)
            {
                // Go back to the settings
                BackToSettings();
            }
        }

        /// <summary>
        /// Reset the key bindings
        /// </summary>
        public void ResetSettings()
        {
            // Loop through each map in the action assets
            foreach(InputActionMap map in inputActionAsset.actionMaps)
            {
                // Remove all bindings
                map.RemoveAllBindingOverrides();
            }

            // Set the default settings for each rebinding button
            foreach(GameRebindingButton rebindingButton in rebindingButtons)
            {
                rebindingButton.SetDefault(inputActionAsset, keysDict);
            }

            // Play the reset button sound
            playButtonReset.Post(gameObject);
        }

        public void Init(Component sender, object data)
        {
            // Get components
            if(controller == null)
                controller = GetComponentInParent<GameSettingsMenu>();

            if(controlSaveHandler == null)
                controlSaveHandler = GetComponent<ControlsSaveHandler>();

            // Clear the animators list
            animators.Clear();

            // Initialize each rebinding button
            foreach (GameRebindingButton rebindingButton in rebindingButtons)
            {
                rebindingButton.Init(this, inputActionAsset, keysDict);
                animators.Add(rebindingButton.GetComponentInChildren<Animator>());
            }

            // Create state machine if not created already
            if (stateMachine == null)
            {
                // Initialize the state machine
                stateMachine = new StateMachine();

                // Construct states
                IdleControlsState idleState = new IdleControlsState(this, animators);
                BindingControlsState bindingState = new BindingControlsState(this, animators);

                // Set state transitions
                At(idleState, bindingState, new FuncPredicate(() => binding));
                At(bindingState, idleState, new FuncPredicate(() => !binding));

                // Set the initial state
                stateMachine.SetState(idleState);
            }
        }
    }
}
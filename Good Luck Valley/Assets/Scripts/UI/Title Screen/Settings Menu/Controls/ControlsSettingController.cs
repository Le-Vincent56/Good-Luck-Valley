using UnityEngine.InputSystem;
using UnityEngine;
using GoodLuckValley.Patterns.StateMachine;
using GoodLuckValley.UI.TitleScreen.Settings.Controls.States;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using GoodLuckValley.Player.Input;
using UnityEngine.UI;
using System.Collections;
using GoodLuckValley.Persistence;

namespace GoodLuckValley.UI.TitleScreen.Settings.Controls
{
    public class ControlsSettingController : SettingsController, IBind<ControlsData>
    {
        [Header("References")]
        [SerializeField] private InputKeyDictionary keysDict;
        [SerializeField] private MenuInputReader menuInputReader;
        [SerializeField] private InputActionAsset inputActionAsset;
        [SerializeField] private Text duplicatesText;
        private ControlsData data;

        [Header("Fields")]
        private const int stateNum = 6;
        [SerializeField] private float fadeDuration = 0.01f;
        [SerializeField] private bool binding;
        [SerializeField] private int currentBindingButton;
        [SerializeField] private List<Animator> animators = new List<Animator>();
        [SerializeField] private List<RebindingButton> rebindingButtons = new List<RebindingButton>();
        private Coroutine duplicateFadeCoroutine;

        private StateMachine stateMachine;
        private InputActionRebindingExtensions.RebindingOperation rebindingOperation;

        public int CurrentRebindingButton { get => currentBindingButton; }
        public string LastRebindingAction { get; private set; }
        [field: SerializeField] public SerializableGuid ID { get; set; } = SerializableGuid.NewGuid();

        protected override void Awake()
        {
            base.Awake();

            // Create state machine
            stateMachine = new StateMachine();

            // Initialize each rebinding button
            foreach (RebindingButton rebindingButton in rebindingButtons)
            {
                rebindingButton.Init(this);
                animators.Add(rebindingButton.GetComponent<Animator>());
            }

            // Construct states
            IdleControlsState idleState = new IdleControlsState(this, animators);
            BindingControlsState bindingState = new BindingControlsState(this, animators);

            // Set state transitions
            At(idleState, bindingState, new FuncPredicate(() => binding));
            At(bindingState, idleState, new FuncPredicate(() => !binding));

            // Set the initial state
            stateMachine.SetState(idleState);
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
        private void RebindSuccessfulComplete(RebindingButton rebindingButton)
        {
            // End the rebinding
            EndRebinding(rebindingButton);

            // Set a valid rebind
            rebindingButton.SetValidRebind(true);
        }

        /// <summary>
        /// Finalize a failed rebinding
        /// </summary>
        /// <param name="action"></param>
        /// <param name="bindingIndex"></param>
        /// <param name="rebindingButton"></param>
        private void RebindFailedComplete(RebindingButton rebindingButton)
        {
            rebindingButton.SetRebinded(true);

            // End the rebinding
            EndRebinding(rebindingButton);

            // Set an invalid rebind
            rebindingButton.SetValidRebind(false);
        }

        /// <summary>
        /// End the rebinding process
        /// </summary>
        private void EndRebinding(RebindingButton rebindingButton)
        {
            rebindingButton.SetRebinded(true);

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
                elapsedTime += Time.deltaTime;
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
                elapsedTime += Time.deltaTime;
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
        public RebindingButton GetCurrentRebindingButton() => rebindingButtons[CurrentRebindingButton];

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
            foreach(RebindingButton rebindingButton in rebindingButtons)
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
                foreach (RebindingButton rebindingButton in rebindingButtons)
                {
                    rebindingButton.SetRebinded(false);
                }

                // Save binding data
                SaveData();

                // Set the settings state
                controller.SetState(controller.SETTINGS);
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
            foreach(RebindingButton rebindingButton in rebindingButtons)
            {
                rebindingButton.SetDefault(inputActionAsset, keysDict);
            }
        }

        public void SaveData()
        {
            data.bindings = inputActionAsset.SaveBindingOverridesAsJson();
            SaveLoadSystem.Instance.SaveSettings();
        }

        public void Bind(ControlsData data, bool applyData = true)
        {
            this.data = data;
            this.data.ID = data.ID;

            inputActionAsset.LoadBindingOverridesFromJson(data.bindings);
        }
    }
}
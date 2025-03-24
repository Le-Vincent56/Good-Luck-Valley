using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.UI.Input;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GoodLuckValley.Input
{
    public class ControlSchemeDetector : SerializedMonoBehaviour
    {
        [Header("References")]
        [SerializeField] InputActionAsset inputActionAsset;
        [SerializeField] private string currentControlScheme;

        [SerializeField] private List<ITransmutableInputUI> transmutableUIs;

        public bool AllowCursor { get; private set; }

        private void Awake()
        {
            // Initialize the list of transmutable UIs to track and process
            transmutableUIs = new List<ITransmutableInputUI>();

            // Register this as a global service
            ServiceLocator.Global.Register(this);
        }

        private void OnEnable()
        {
            // Iterate through each Action Map
            foreach (InputActionMap actionMap in inputActionAsset.actionMaps)
            {
                // Iterate through each Action in the Action Map
                foreach (InputAction action in actionMap)
                {
                    // Add the action performed event
                    action.performed += OnActionPerformed;
                }
            }

            // Initialize the control scheme
            UpdateControlScheme();
        }

        private void OnDisable()
        {
            // Iterate through each Action Map
            foreach (InputActionMap actionMap in inputActionAsset.actionMaps)
            {
                // Iterate through each Action in the Action Map
                foreach (InputAction action in actionMap)
                {
                    // Remove the action performed event
                    action.performed -= OnActionPerformed;
                }
            }
        }

        /// <summary>
        /// Callback action to check the current control scheme
        /// </summary>
        private void OnActionPerformed(InputAction.CallbackContext context) => UpdateControlScheme(context.control.device);

        /// <summary>
        /// Update the control scheme based on the given device
        /// </summary>
        private void UpdateControlScheme(InputDevice device = null)
        {
            // Prioritize the last-used device
            InputDevice lastDevice = InputSystem.devices.FirstOrDefault(
                device => device.lastUpdateTime == InputSystem.devices.Max(
                    deviceTwo => deviceTwo.lastUpdateTime
                )
            );

            // If there's a last device, set the last device as the updating device
            if (lastDevice != null)
            {
                SetControlScheme(lastDevice);
                return;
            }

            // If there's no last device, set the given device as the updating device
            if (device != null)
            {
                SetControlScheme(device);
                return;
            }

            // Default the control scheme to Keyboard & Mouse
            currentControlScheme = inputActionAsset.controlSchemes[0].name;

            // Process the control scheme
            ProcessControlScheme();
        }

        /// <summary>
        /// Set the control scheme based on a given device
        /// </summary>
        private void SetControlScheme(InputDevice device)
        {
            // Iterate through each control scheme
            foreach (InputControlScheme controlScheme in inputActionAsset.controlSchemes)
            {
                // Skip if the control scheme does not support the given device
                if (!controlScheme.SupportsDevice(device)) continue;

                // Set the first supporting control scheme
                currentControlScheme = controlScheme.name;
                break;
            }

            // Process the control scheme
            ProcessControlScheme();
        }

        /// <summary>
        /// Process the control scheme by updating all transmutable UIs
        /// </summary>
        private void ProcessControlScheme()
        {
            // Iterate through each transmutable UI
            foreach (ITransmutableInputUI transmutableInputUI in transmutableUIs)
            {
                // Transmute them with the current control scheme
                transmutableInputUI.Transmute(currentControlScheme);
            }

            switch (currentControlScheme)
            {
                case "Keyboard and Mouse":
                    AllowCursor = true;
                    break;
                case "Xbox Controller":
                    AllowCursor = false;
                    break;
                case "PlayStation":
                    AllowCursor = false;
                    break;
            }
        }

        /// <summary>
        /// Register a Transmutable Input UI to be tracked and processed
        /// </summary>
        public void Register(ITransmutableInputUI transmutableUI)
        {
            // Exit case - the transmutable input UI is already registered
            if (transmutableUIs.Contains(transmutableUI)) return;

            // Add the transmutable input UI to the list
            transmutableUIs.Add(transmutableUI);

            // Process the control scheme
            ProcessControlScheme();
        }

        /// <summary>
        /// Deregister a Transmutable Input UI to be tracked and processed
        /// </summary>
        public void Deregister(ITransmutableInputUI transmutableUI)
        {
            // Exit case - the transmutable input UI is not registered
            if (!transmutableUIs.Contains(transmutableUI)) return;

            // Remove the transmutable input UI from the list
            transmutableUIs.Remove(transmutableUI);
        }
    }
}

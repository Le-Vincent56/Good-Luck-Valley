using GoodLuckValley.Architecture.ServiceLocator;
using GoodLuckValley.UI.Input;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace GoodLuckValley.Input
{
    public class ControlSchemeDetector : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] InputActionAsset inputActionAsset;
        [SerializeField] private string currentControlScheme;

        private List<ITransmutableInputUI> transmutableUIs;

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
            // Check if there's no device
            if (device == null)
            {
                // Fallback on the last used device
                InputDevice lastDevice = InputSystem.devices.FirstOrDefault(
                    device => device.lastUpdateTime == InputSystem.devices.Max(
                        deviceTwo => deviceTwo.lastUpdateTime
                    )
                );

                // Check if there was a last used device
                if (lastDevice != null)
                {
                    // Update the control scheme
                    UpdateControlScheme(lastDevice);
                    return;
                }

                // Default the control scheme to Keyboard & Mouse
                currentControlScheme = inputActionAsset.controlSchemes[0].name;
            }

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

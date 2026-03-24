using GoodLuckValley.Core.Input.Interfaces;
using GoodLuckValley.Core.Input.Services;
using UnityEngine;

namespace GoodLuckValley.Core.Input.Adapters
{
    /// <summary>                                                                        
    /// Thin MonoBehaviour bridge between Unity's Input System and InputService.         
    /// Subscribes to InputSystem_Actions callbacks, forwards values to the pure C# service.
    /// Implements IInputMapSwitcher so InputService can request map changes without
    /// depending on MonoBehaviour types.
    /// Lives on a persistent DontDestroyOnLoad GameObject created by the Bootstrapper.
    /// </summary>
    public class InputAdapter : MonoBehaviour, IInputMapSwitcher
    {
        private InputSystem_Actions _actions;
        private InputService _inputService;

        private void OnDestroy()
        {
            if (_actions == null) return;
            
            _actions.Player.Disable();
            _actions.UI.Disable();
            _actions.Dispose();
            _actions = null;
        }

        /// <summary>
        /// Initializes the input adapter with the given input service, sets up input mappings, and activates the default input map.
        /// </summary>
        /// <param name="inputService">The input service that provides input context and map switching functionality.</param>
        public void Initialize(InputService inputService)
        {
            _inputService = inputService;
            _inputService.SetMapSwitcher(this);

            _actions = new InputSystem_Actions();

            SubscribePlayerActions();
            SubscribeUIActions();

            // Start in Player context by default
            EnablePlayerMap();
        }

        /// <summary>
        /// Enables the Player input map by activating player-specific controls and disabling UI controls.
        /// </summary>
        public void EnablePlayerMap()
        {
            _actions.UI.Disable();
            _actions.Player.Enable();
        }

        /// <summary>
        /// Activates the UI input map and disables the player input map, ensuring that only UI-related input actions are processed.
        /// </summary>
        public void EnableUIMap()
        {
            _actions.Player.Disable();
            _actions.UI.Enable();
        }

        /// <summary>
        /// Subscribes player-specific input actions from the input system to corresponding service methods.
        /// This includes handling both continuous actions like movement, buffered press actions like jump or interact,
        /// and held actions like crouch.
        /// </summary>
        private void SubscribePlayerActions()
        {
            // Continuous
            _actions.Player.Move.performed += (ctx) => _inputService.SetMove(ctx.ReadValue<Vector2>());
            _actions.Player.Move.canceled += (ctx) => _inputService.SetMove(Vector2.zero);

            // Buffered press
            _actions.Player.Jump.performed += (ctx) => _inputService.OnJumpPerformed();
            _actions.Player.Jump.canceled += (ctx) => _inputService.OnJumpCanceled();
            _actions.Player.Bounce.performed += (ctx) => _inputService.OnBouncePerformed();
            _actions.Player.Interact.performed += (ctx) => _inputService.OnInteractPerformed();
            _actions.Player.Previous.performed += (ctx) => _inputService.OnPreviousPerformed();
            _actions.Player.Next.performed += (ctx) => _inputService.OnNextPerformed();

            // Held
            _actions.Player.Crouch.performed += (ctx) => _inputService.OnCrouchPerformed();
            _actions.Player.Crouch.canceled += (ctx) => _inputService.OnCrouchCanceled();
        }

        /// <summary>
        /// Subscribes UI-related input actions to the corresponding handlers in the input service.
        /// This includes actions such as navigation, submit, and cancel, ensuring that UI inputs are processed and handled appropriately.
        /// </summary>
        private void SubscribeUIActions()
        {
            _actions.UI.Navigate.performed += (ctx) => _inputService.SetNavigate(ctx.ReadValue<Vector2>());
            _actions.UI.Navigate.canceled += (ctx) => _inputService.SetNavigate(Vector2.zero);

            _actions.UI.Submit.performed += (ctx) => _inputService.OnSubmitPerformed();
            _actions.UI.Cancel.performed += (ctx) => _inputService.OnCancelPerformed();
        }
    }
}
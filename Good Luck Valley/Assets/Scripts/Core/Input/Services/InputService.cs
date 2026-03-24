using System;
using UnityEngine;
using GoodLuckValley.Core.Input.Enums;
using GoodLuckValley.Core.Input.Interfaces;

namespace GoodLuckValley.Core.Input.Services
{
    public class InputService : IPlayerInput, IUIInput, IInputContextService
    {
        private const float BufferDuration = 0.15f;
        private readonly Func<float> _timeProvider;

        // Continuous values
        private Vector2 _move;
        private Vector2 _navigate;
        
        // Held values
        private bool _crouchHeld;
        private bool _jumpHeld;
        
        // Buffered press state
        private float _jumpPressTime = -1f;
        private bool _jumpConsumed = true;

        private float _bouncePressTime = -1f;
        private bool _bounceConsumed = true;

        private float _interactPressTime = -1f;
        private bool _interactConsumed = true;

        private float _previousPressTime = -1f;
        private bool _previousConsumed = true;

        private float _nextPressTime = -1f;
        private bool _nextConsumed = true;

        private float _submitPressTime = -1f;
        private bool _submitConsumed = true;

        private float _cancelPressTime = -1f;
        private bool _cancelConsumed = true;
        
        // Context switching
        private InputContext _currentContext = InputContext.Player;
        private IInputMapSwitcher _mapSwitcher;

        public Vector2 Move => _move;
        public Vector2 Navigate => _navigate;
        public bool CrouchHeld => _crouchHeld;
        public bool JumpHeld => _jumpHeld;
        public bool JumpPressed
        {
            get
            {
                if (_jumpConsumed) return false;
                if (_timeProvider() - _jumpPressTime > BufferDuration) return false;
                _jumpConsumed = true;
                return true;
            }
        }

        public bool BouncePressed
        {
            get
            {
                if (_bounceConsumed) return false;
                if (_timeProvider() - _bouncePressTime > BufferDuration) return false;
                _bounceConsumed = true;
                return true;
            }
        }

        public bool InteractPressed
        {
            get
            {
                if (_interactConsumed) return false;
                if (_timeProvider() - _interactPressTime > BufferDuration) return false;
                _interactConsumed = true;
                return true;
            }
        }

        public bool PreviousPressed
        {
            get
            {
                if (_previousConsumed) return false;
                if (_timeProvider() - _previousPressTime > BufferDuration) return false;
                _previousConsumed = true;
                return true;
            }
        }

        public bool NextPressed
        {
            get
            {
                if (_nextConsumed) return false;
                if (_timeProvider() - _nextPressTime > BufferDuration) return false;
                _nextConsumed = true;
                return true;
            }
        }

        public bool SubmitPressed
        {
            get
            {
                if (_submitConsumed) return false;
                if (_timeProvider() - _submitPressTime > BufferDuration) return false;
                _submitConsumed = true;
                return true;
            }
        }

        public bool CancelPressed
        {
            get
            {
                if (_cancelConsumed) return false;
                if (_timeProvider() - _cancelPressTime > BufferDuration) return false;
                _cancelConsumed = true;
                return true;
            }
        }
        
        public InputContext CurrentContext => _currentContext;
        public event Action<InputContext> OnContextChanged;

        public InputService(Func<float> timeProvider) => _timeProvider = timeProvider;

        /// <summary>
        /// Sets the movement input value for the player.
        /// Updates the internal state to reflect the directional input provided.
        /// </summary>
        /// <param name="value">A 2D vector representing the movement direction and magnitude (e.g., from input devices).</param>
        public void SetMove(Vector2 value) => _move = value;

        /// <summary>
        /// Sets the navigation input value.
        /// Updates the internal state to reflect the directional input intended for navigation purposes.
        /// </summary>
        /// <param name="value">A 2D vector representing the navigation direction and magnitude (e.g., from input devices).</param>
        public void SetNavigate(Vector2 value) => _navigate = value;

        /// <summary>
        /// Sets the active input context, determining whether the input system
        /// processes inputs for the player or the UI.
        /// Clears all input state and invokes context-specific input map switches.
        /// Triggers the context change event.
        /// </summary>
        /// <param name="context">The new input context to be set, either Player or UI.</param>
        public void SetContext(InputContext context)
        {
            if (context == _currentContext) return;

            _currentContext = context;
            ClearAllState();

            if (_mapSwitcher != null)
            {
                switch (context)
                {
                    case InputContext.Player:
                        _mapSwitcher.EnablePlayerMap();
                        break;
                    
                    case InputContext.UI:
                        _mapSwitcher.EnableUIMap();
                        break;
                }
            }

            OnContextChanged?.Invoke(context);
        }

        /// <summary>
        /// Handles the action to be performed when the "crouch" input is performed.
        /// Sets the crouch state to indicate that the crouch input is currently held.
        /// </summary>
        public void OnCrouchPerformed() => _crouchHeld = true; 

        /// <summary>
        /// Handles the action to be performed when the "crouch" input is canceled.
        /// Resets the crouch state to indicate that the crouch input is no longer held.
        /// </summary>
        public void OnCrouchCanceled() => _crouchHeld = false;

        /// <summary>
        /// Handles the action to be performed when the "jump" input is triggered.
        /// Records the time of the jump input and resets its consumed state, allowing the jump input
        /// to be registered and processed within the valid input buffer duration.
        /// </summary>
        public void OnJumpPerformed()
        {
            _jumpPressTime = _timeProvider();
            _jumpConsumed = false;
            _jumpHeld = true;
        }

        /// <summary>
        /// Cancels the jump input action for the player.
        /// Updates the internal state to indicate that the jump is no longer being held.
        /// </summary>
        public void OnJumpCanceled() => _jumpHeld = false;

        /// <summary>
        /// Handles the action to be performed when the "bounce" input is triggered.
        /// Updates the system by recording the time of the input and resetting the consumed state,
        /// enabling the "bounce" input to be processed within the buffer time window.
        /// </summary>
        public void OnBouncePerformed()
        {
            _bouncePressTime = _timeProvider();
            _bounceConsumed = false;
        }

        /// <summary>
        /// Handles the action to be performed when the "interact" input is triggered.
        /// Updates the system by recording the time of the input and resetting the consumed state,
        /// allowing the "interact" input to be processed within the buffer time window.
        /// </summary>
        public void OnInteractPerformed()
        {
            _interactPressTime = _timeProvider();
            _interactConsumed = false;
        }

        /// <summary>
        /// Handles the action to be performed when the "previous" input is triggered.
        /// Updates the internal state by recording the time of the input and resetting the consumed state,
        /// allowing the "previous" input to be processed.
        /// </summary>
        public void OnPreviousPerformed()
        {
            _previousPressTime = _timeProvider();
            _previousConsumed = false;
        }

        /// <summary>
        /// Handles the action to be performed when the "next" input is triggered.
        /// Updates the internal state by recording the time of the input and resetting the consumed state,
        /// allowing the "next" input to be processed.
        /// </summary>
        public void OnNextPerformed()
        {
            _nextPressTime = _timeProvider();
            _nextConsumed = false;
        }

        /// <summary>
        /// Handles the action to be performed when a submit input is triggered.
        /// Updates the internal state to record the time the submit action was performed
        /// and resets the consumed state, allowing the submit input to be processed.
        /// </summary>
        public void OnSubmitPerformed()
        {
            _submitPressTime = _timeProvider();
            _submitConsumed = false;
        }

        /// <summary>
        /// Handles the action to be performed when a cancel input is triggered.
        /// Updates the internal state to record the time the cancel action was performed
        /// and resets the consumed state, allowing the cancel input to be processed.
        /// </summary>
        public void OnCancelPerformed()
        {
            _cancelPressTime = _timeProvider();
            _cancelConsumed = false;
        }

        /// <summary>
        /// Sets the input map switcher for managing context transitions.
        /// Assigns the provided switcher to enable toggling between input maps as required.
        /// </summary>
        /// <param name="switcher">An implementation of IInputMapSwitcher used for switching between input action maps.</param>
        public void SetMapSwitcher(IInputMapSwitcher switcher) => _mapSwitcher = switcher;

        /// <summary>
        /// Clears all input states, resetting continuous values to zero,
        /// disabling held states, and marking all buffered inputs as consumed.
        /// This ensures a clean slate for input tracking.
        /// </summary>
        private void ClearAllState()
        {
            // Continuous
            _move = Vector2.zero;
            _navigate = Vector2.zero;

            // Held
            _crouchHeld = false;
            _jumpHeld = false;

            // Buffered — mark all as consumed
            _jumpConsumed = true;
            _bounceConsumed = true;
            _interactConsumed = true;
            _previousConsumed = true;
            _nextConsumed = true;
            _submitConsumed = true;
            _cancelConsumed = true;
        }
    }
}
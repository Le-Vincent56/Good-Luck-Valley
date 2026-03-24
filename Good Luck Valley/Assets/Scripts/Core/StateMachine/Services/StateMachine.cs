using System;
using System.Collections.Generic;
using GoodLuckValley.Core.StateMachine.Exceptions;
using GoodLuckValley.Core.StateMachine.Interfaces;

namespace GoodLuckValley.Core.StateMachine.Services
{
    /// <summary>
    /// Generic finite state machine that manages state lifecycle, type-based
    /// transitions, and state change notification. Implements
    /// <see cref="IStateMachine{TState}"/> for the owning service and
    /// <see cref="IStateMachineContext{TState}"/> for states.
    /// </summary>
    /// <typeparam name="TState">The state base type (must implement IState).</typeparam>
    public class StateMachine<TState> : IStateMachine<TState>, IStateMachineContext<TState>
        where TState : IState
    {
        private const int MaxTransitionsPerFrame = 10;

        private readonly Dictionary<Type, TState> _states = new Dictionary<Type, TState>();
        private TState _currentState;
        private Type _currentStateType;
        private bool _isStarted;
        private int _transitionCount;

        /// <summary>
        /// The currently active state. Null before Start() is called.
        /// </summary>
        public TState CurrentState => _currentState;

        /// <summary>
        /// Fired after a state transition completes (Exit on old state, Enter on new state).
        /// Parameters are (previousState, currentState).
        /// </summary>
        public event Action<TState, TState> OnStateChanged;

        /// <summary>
        /// Registers a state instance, keyed by its concrete type.
        /// Must be called before Start(). Throws if a state of this type
        /// is already registered or if Start() has already been called.
        /// </summary>
        /// <typeparam name="T">The concrete type of the state to register.</typeparam>
        /// <param name="state">The state instance to register.</param>
        public void AddState<T>(T state) where T : TState
        {
            if (state == null)
                throw new StateMachineException("Cannot register a null state.");

            Type stateType = typeof(T);

            if (_isStarted)
            {
                throw new StateMachineException(
                    $"Cannot add state '{stateType.Name}' after the state machine has started. " +
                    $"Register all states before calling Start()."
                );
            }

            if (!_states.TryAdd(stateType, state))
                throw new StateMachineException($"A state of type '{stateType.Name}' is already registered.");
        }

        /// <summary>
        /// Sets the initial state and calls its Enter(). All states must be
        /// registered via AddState before calling Start(). Throws if called
        /// more than once or if the type is not registered.
        /// </summary>
        /// <typeparam name="T">The concrete type of the initial state.</typeparam>
        public void Start<T>() where T : TState
        {
            if(_isStarted)
                throw new StateMachineException("Start() has already been called. The state machine can only be started once.");
            
            Type stateType = typeof(T);

            if (!_states.TryGetValue(stateType, out TState state))
            {
                throw new StateMachineException(
                    $"Cannot start with state '{stateType.Namespace}' - it has not been registered. " +
                    $"Call AddState<T>() before State<T>()."
                );
            }

            _isStarted = true;
            _currentState = state;
            _currentStateType = stateType;
            _currentState.Enter();
        }

        /// <summary>
        /// Forwards the Update tick to the current state. Resets the
        /// per-frame transition counter. No-op if Start() has not been called.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last frame.</param>
        public void Update(float deltaTime)
        {
            if (!_isStarted) return;

            _transitionCount = 0;
            _currentState.Update(deltaTime);
        }

        /// <summary>
        /// Forwards the FixedUpdate tick to the current state. Does not reset
        /// the transition counter. No-op if Start() has not been called.
        /// </summary>
        /// <param name="fixedDeltaTime">Fixed timestep duration.</param>
        public void FixedUpdate(float fixedDeltaTime)
        {
            if (!_isStarted) return;
            
            _currentState.FixedUpdate(fixedDeltaTime);
        }

        /// <summary>
        /// Requests an immediate transition to the state registered as type T.
        /// Calls Exit() on the current state and Enter() on the new state.
        /// No-op if the current state is already of type T.
        /// </summary>
        /// <typeparam name="T">The concrete type of the target state.</typeparam>
        public void ChangeState<T>() where T : TState
        {
            Type stateType = typeof(T);

            if (!_states.TryGetValue(stateType, out TState newState))
            {
                throw new StateMachineException(
                    $"Cannot transition to state '{stateType.Name} - it has not been registered. " +
                    $"Call AddState<T>() before transitioning to it."
                );
            }
            
            // No-op if already in this state (compared by registered type key)
            if (stateType == _currentStateType) return;

            _transitionCount++;

            if (_transitionCount > MaxTransitionsPerFrame)
            {
                throw new StateMachineException(
                    $"Exceeded maximum transitions per frame ({MaxTransitionsPerFrame}). " +
                    $"This likely indicates an infinite transition loop between states."
                );
            }

            TState previous = _currentState;
            _currentState.Exit();
            _currentState = newState;
            _currentStateType = stateType;
            _currentState.Enter();
            OnStateChanged?.Invoke(previous, _currentState);
        }
    }
}
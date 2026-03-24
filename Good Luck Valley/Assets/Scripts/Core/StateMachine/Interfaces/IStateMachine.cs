using System;

  namespace GoodLuckValley.Core.StateMachine.Interfaces
  {
      /// <summary>
      /// Owner-facing interface for the finite state machine. The owning service
      /// uses this to register states, start the machine, tick it, and observe
      /// state changes. States do not see this interface — they receive
      /// <see cref="IStateMachineContext{TState}"/> instead.
      /// </summary>
      /// <typeparam name="TState">The state base type for this state machine.</typeparam>
      public interface IStateMachine<TState> where TState : IState
      {
          /// <summary>
          /// The currently active state. Null before Start() is called.
          /// </summary>
          TState CurrentState { get; }
          
          /// <summary>
          /// Fired after a state transition completes (Exit on old state, Enter on new state).
          /// Parameters are (previousState, currentState).
          /// </summary>
          event Action<TState, TState> OnStateChanged;
          
          /// <summary>
          /// Registers a state instance, keyed by its concrete type.
          /// Must be called before Start(). Throws if a state of this type
          /// is already registered or if Start() has already been called.
          /// </summary>
          /// <typeparam name="T">The concrete type of the state to register.</typeparam>
          /// <param name="state">The state instance to register.</param>
          void AddState<T>(T state) where T : TState;

          /// <summary>
          /// Sets the initial state and calls its Enter(). All states must be
          /// registered via AddState before calling Start(). Throws if called
          /// more than once or if the type is not registered.
          /// </summary>
          /// <typeparam name="T">The concrete type of the initial state.</typeparam>
          void Start<T>() where T : TState;

          /// <summary>
          /// Forwards the Update tick to the current state. Resets the
          /// per-frame transition counter. No-op if Start() has not been called.
          /// </summary>
          /// <param name="deltaTime">Time elapsed since the last frame.</param>
          void Update(float deltaTime);

          /// <summary>
          /// Forwards the FixedUpdate tick to the current state. Does not reset
          /// the transition counter. No-op if Start() has not been called.
          /// </summary>
          /// <param name="fixedDeltaTime">Fixed timestep duration.</param>
          void FixedUpdate(float fixedDeltaTime);
      }
  }
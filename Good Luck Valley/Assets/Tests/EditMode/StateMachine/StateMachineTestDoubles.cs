using System;
using System.Collections.Generic;
using GoodLuckValley.Core.StateMachine.Interfaces;

namespace GoodLuckValley.Tests.EditMode.StateMachine
{
    /// <summary>
    /// Base test state interface used as the TState constraint in tests.
    /// </summary>
    public interface ITestState : IState { }

    /// <summary>
    /// Stub state that tracks all lifecycle calls. Optionally triggers a
    /// transition during Update or Enter via configurable callbacks.
    /// </summary>
    public class StubState : ITestState
    {
        private readonly IStateMachineContext<ITestState> _context;
        private Action _onEnterAction;
        private Action _onUpdateAction;
        private Action _onFixedUpdateAction;

        public int EnterCount { get; private set; }
        public int ExitCount { get; private set; }
        public int UpdateCount { get; private set; }
        public int FixedUpdateCount { get; private set; }
        public float LastDeltaTime { get; private set; }
        public float LastFixedDeltaTime { get; private set; }
        public List<string> CallLog { get; } = new List<string>();

        public StubState(IStateMachineContext<ITestState> context)
        {
            _context = context;
        }

        /// <summary>
        /// Configures an action to execute during Enter (e.g., trigger a transition).
        /// </summary>
        public void SetOnEnterAction(Action action) => _onEnterAction = action;

        /// <summary>
        /// Configures an action to execute during Update (e.g., trigger a transition).
        /// </summary>
        public void SetOnUpdateAction(Action action) => _onUpdateAction = action;

        /// <summary>
        /// Configures an action to execute during FixedUpdate.
        /// </summary>
        public void SetOnFixedUpdateAction(Action action)
        {
            _onFixedUpdateAction = action;
        }

        public void Enter()
        {
            EnterCount++;
            CallLog.Add("Enter");
            _onEnterAction?.Invoke();
        }

        public void Exit()
        {
            ExitCount++;
            CallLog.Add("Exit");
        }

        public void Update(float deltaTime)
        {
            UpdateCount++;
            LastDeltaTime = deltaTime;
            CallLog.Add("Update");
            _onUpdateAction?.Invoke();
        }

        public void FixedUpdate(float fixedDeltaTime)
        {
            FixedUpdateCount++;
            LastFixedDeltaTime = fixedDeltaTime;
            CallLog.Add("FixedUpdate");
            _onFixedUpdateAction?.Invoke();
        }
    }

    // --- Concrete state types for type-based registration ---
    // Each is a distinct type, so AddState<T> keys them differently.

    public class StateA : StubState
    {
        public StateA(IStateMachineContext<ITestState> context) : base(context)
        { }
    }

    public class StateB : StubState
    {
        public StateB(IStateMachineContext<ITestState> context) : base(context)
        { }
    }

    public class StateC : StubState
    {
        public StateC(IStateMachineContext<ITestState> context) : base(context)
        { }
    }

    /// <summary>
    /// A state type that is never registered — used to test "not registered" errors.
    /// </summary>
    public class UnregisteredState : StubState
    {
        public UnregisteredState(IStateMachineContext<ITestState> context) : base(context)
        { }
    }

    /// <summary>
    /// Tracks OnStateChanged event invocations for assertions.
    /// </summary>
    public class StateChangeTracker
    {
        public List<(ITestState Previous, ITestState Current)> Changes { get; } = new List<(ITestState, ITestState)>();

        public void OnStateChanged(ITestState previous, ITestState current)
        {
            Changes.Add((previous, current));
        }
    }
}
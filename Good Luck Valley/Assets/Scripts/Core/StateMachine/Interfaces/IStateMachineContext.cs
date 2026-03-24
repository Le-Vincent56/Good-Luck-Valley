namespace GoodLuckValley.Core.StateMachine.Interfaces
{
    /// <summary>
    /// Narrow interface provided to states for requesting transitions.
    /// States receive this via constructor — they can request transitions
    /// but cannot tick the FSM, read its state, or subscribe to events.
    /// </summary>
    /// <typeparam name="TState">The state base type for this state machine.</typeparam>
    public interface IStateMachineContext<TState> where TState : IState
    {
        /// <summary>
        /// Requests an immediate transition to the state registered as type T.
        /// Calls Exit() on the current state and Enter() on the new state.
        /// No-op if the current state is already of type T.
        /// </summary>
        /// <typeparam name="T">The concrete type of the target state.</typeparam>
        void ChangeState<T>() where T : TState;
    }
}
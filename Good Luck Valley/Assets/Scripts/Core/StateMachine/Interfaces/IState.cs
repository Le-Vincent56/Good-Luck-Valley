namespace GoodLuckValley.Core.StateMachine.Interfaces
{
    /// <summary>
    /// Contract for a state in a finite state machine. Each state encapsulates its own behavior
    /// (Update/FixedUpdate) and lifecycle (Enter/Exit)
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// Called when this state becomes the active state.
        /// </summary>
        void Enter();

        /// <summary>
        /// Called when this state is no longer the active state.
        /// </summary>
        void Exit();

        /// <summary>
        /// Called once per frame by the state machine's Update tick.
        /// </summary>
        /// <param name="deltaTime">Time elapsed since the last frame.</param>
        void Update(float deltaTime);

        /// <summary>
        /// Called once per fixed timestep by the state machine's FixedUpdate tick.
        /// </summary>
        /// <param name="fixedDeltaTime">Fixed timestep duration.</param>
        void FixedUpdate(float fixedDeltaTime);
    }
}
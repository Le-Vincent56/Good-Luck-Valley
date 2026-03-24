using GoodLuckValley.Core.StateMachine.Interfaces;

namespace GoodLuckValley.Gameplay.Player.States
{
    /// <summary>
    /// Represents a sub-state that operates while the player is in a grounded state.
    /// This interface extends the IState interface, ensuring the implementation
    /// inherits standard state methods such as Enter, Exit, Update, and FixedUpdate.
    /// </summary>
    public interface IGroundSubState : IState { }
}
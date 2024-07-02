namespace GoodLuckValley.Patterns.StateMachine
{
    /// <summary>
    /// A Transition requires a State to go to
    /// and a Predicate on when to go to that State
    /// </summary>
    public interface ITransition
    {
        IState To { get; }
        IPredicate Condition { get; }
    }
}

namespace GoodLuckValley.Architecture.StateMachine
{
    /// <summary>
    /// A Predicate is an expression that returns true or false depending
    /// on an evaluation
    /// </summary>
    public interface IPredicate
    {
        /// <summary>
        /// Evaluate the Predicate
        /// </summary>
        /// <returns>True if the predicate is true, false if not</returns>
        bool Evaluate();
    }
}
using System;

namespace GoodLuckValley.Architecture.StateMachine
{
    public class FuncPredicate : IPredicate
    {
        readonly Func<bool> func;

        public FuncPredicate(Func<bool> func)
        {
            this.func = func;
        }

        /// <summary>
        /// Evaluate the predicate
        /// </summary>
        public bool Evaluate() => func.Invoke();
    }
}
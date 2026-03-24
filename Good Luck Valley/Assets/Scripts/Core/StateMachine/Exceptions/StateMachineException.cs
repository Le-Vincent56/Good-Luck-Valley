using System;

namespace GoodLuckValley.Core.StateMachine.Exceptions
{
    /// <summary>                                                                     
    /// Exception thrown when a state machine operation fails.                     
    /// </summary>
    public class StateMachineException : Exception
    {
        public StateMachineException(string message) : base(message) { }

        public StateMachineException(string message, Exception innerException) : base(message, innerException) 
        { }
    }
}
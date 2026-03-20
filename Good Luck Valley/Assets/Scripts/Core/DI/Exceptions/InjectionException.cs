using System;

namespace GoodLuckValley.Core.DI.Exceptions
{
    /// <summary>
    /// Base exception for all dependency injection errors.
    /// </summary>
    public class InjectionException : Exception
    {
        /// <summary>
        /// Represents a base exception for errors that occur during dependency injection operations.
        /// </summary>
        public InjectionException(string message) : base(message) { }

        /// <summary>
        /// Represents a base exception for errors that occur during dependency injection operations.
        /// </summary>
        public InjectionException(string message, Exception innerException) : base(message, innerException) { }
    }
}
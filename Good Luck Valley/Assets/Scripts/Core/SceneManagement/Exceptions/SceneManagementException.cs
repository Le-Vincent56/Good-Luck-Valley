using System;

namespace GoodLuckValley.Core.SceneManagement.Exceptions
{
    /// <summary>
    /// Exception thrown when a scene management operation fails.
    /// </summary>
    public class SceneManagementException : Exception
    {
        public SceneManagementException(string message) : base(message) { }
        
        public SceneManagementException(string message, Exception innerException)
            : base(message, innerException) 
        { }
    }
}
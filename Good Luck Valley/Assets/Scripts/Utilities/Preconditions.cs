using GoodLuckValley.Extensions.GameObjects;
using System;

namespace GoodLuckValley.Utilities.Preconditions
{
    public class Preconditions
    {
        public Preconditions() { }

        /// <summary>
        /// Check if the referenced object is null
        /// </summary>
        public static T CheckNotNull<T>(T reference) where T : UnityEngine.Object
        {
            return CheckNotNull(reference, null);
        }

        /// <summary>
        /// Check if the referenced object is null with a custom message
        /// </summary>
        public static T CheckNotNull<T>(T reference, string message) where T : UnityEngine.Object
        {
            if (reference.OrNull() == null)
            {
                throw new ArgumentNullException(message);
            }
            if (reference is null)
            {
                throw new ArgumentNullException(message);
            }
            return reference;
        }

        /// <summary>
        /// Check the state of an expression
        /// </summary>
        public static void CheckState(bool expression)
        {
            CheckState(expression, null);
        }

        /// <summary>
        /// Check the state of an expression with a templated message
        /// </summary>
        public static void CheckState(bool expression, string messageTemplate, params object[] messageArgs)
        {
            CheckState(expression, string.Format(messageTemplate, messageArgs));
        }

        /// <summary>
        /// Evaluate an expression and throw a message if false
        /// </summary>
        public static void CheckState(bool expression, string message)
        {
            if (expression)
            {
                return;
            }

            throw message == null ? new InvalidOperationException() : new InvalidOperationException(message);
        }
    }
}
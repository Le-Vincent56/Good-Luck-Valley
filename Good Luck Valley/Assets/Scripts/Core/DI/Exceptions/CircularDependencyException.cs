using System;
using System.Collections.Generic;

namespace GoodLuckValley.Core.DI.Exceptions
{
    /// <summary>
    /// Thrown when the container detects a circular dependency during resolution.
    /// The message includes the full resolution chain for debugging.
    /// </summary>
    public sealed class CircularDependencyException : InjectionException
    {
        /// <summary>
        /// The ordered resolution chain that formed the cycle, ending with the repeated type.
        /// </summary>
        public IReadOnlyList<Type> ResolutionChain { get; }

        public CircularDependencyException(IReadOnlyList<Type> resolutionChain, Type repeatedType)
            : base(FormatMessage(resolutionChain, repeatedType))
        {
            List<Type> chain = new List<Type>(resolutionChain) { repeatedType };
            ResolutionChain = chain.AsReadOnly();
        }

        /// <summary>
        /// Formats a detailed error message describing the detected circular dependency in the resolution chain.
        /// </summary>
        /// <param name="chain">The sequence of types involved in the resolution leading up to the circular dependency.</param>
        /// <param name="repeatedType">The type that caused the circular dependency by appearing multiple times in the resolution chain.</param>
        /// <returns>A formatted string describing the circular dependency, including the full resolution chain for debugging purposes.</returns>
        private static string FormatMessage(IReadOnlyList<Type> chain, Type repeatedType)
        {
            List<string> names = new List<string>(chain.Count + 1);

            foreach (Type type in chain)
            {
                names.Add(type.Name);
            }
            
            names.Add(repeatedType.Name);
            
            return $"Circular dependency detected: {string.Join(" -> ", names)}";
        }
    }
}
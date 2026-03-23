using System;
using System.Text;

namespace GoodLuckValley.Core.Utilities
{
    /// <summary>
    /// Provides deterministic hashing for stable integer IDs. Uses FNV-1a (32-bit),
    /// which produces consistent results across platforms and runtimes. Intended for
    /// edit-time baking — hash values are stored in ScriptableObjects, not computed at runtime.
    /// </summary>
    public static class HashUtility
    {
        private const uint FnvOffsetBasis = 2166136261;
        private const uint FnvPrime = 16777619;

        /// <summary>
        /// Computes a deterministic 32-bit hash from the given string using FNV-1a.
        /// The result is stable across platforms, runtimes, and sessions.
        /// </summary>
        /// <param name="input">The string to hash. Must not be null.</param>
        /// <returns>A stable 32-bit integer hash of the input.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="input"/> is null.</exception>
        public static int ComputeStableHash(string input)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            byte[] bytes = Encoding.UTF8.GetBytes(input);
            uint hash = FnvOffsetBasis;

            for (int i = 0; i < bytes.Length; i++)
            {
                hash ^= bytes[i];
                hash *= FnvPrime;
            }

            return unchecked((int)hash);
        }
    }
}
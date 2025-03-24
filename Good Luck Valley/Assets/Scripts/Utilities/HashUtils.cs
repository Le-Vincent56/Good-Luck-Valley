using System;
using System.Security.Cryptography;

namespace GoodLuckValley.Utilities.Hash
{
    public static class HashUtils
    {
        private static readonly RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        /// <summary>
        /// Generate a random Hash
        /// </summary>
        public static int GenerateHash()
        {
            // Get a unique set of random bytes
            byte[] randomBytes = new byte[16];
            rng.GetBytes(randomBytes);

            // Use SHA256 for hash generation
            using (SHA256 sha256 = SHA256.Create())
            {
                // Compute the hash
                byte[] hashBytes = sha256.ComputeHash(randomBytes);

                // Convert the bytes to an int
                int hashInt = BitConverter.ToInt32(hashBytes, 0);

                return hashInt;
            }
        }
    }
}
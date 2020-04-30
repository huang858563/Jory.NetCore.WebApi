﻿using System;
using System.Security.Cryptography;

namespace Jory.NetCore.WebApi.Common
{
    internal class InvalidHashException : Exception
    {
        public InvalidHashException() { }
        public InvalidHashException(string message)
            : base(message) { }
        public InvalidHashException(string message, Exception inner)
            : base(message, inner) { }
    }

    internal class CannotPerformOperationException : Exception
    {
        public CannotPerformOperationException() { }
        public CannotPerformOperationException(string message)
            : base(message) { }
        public CannotPerformOperationException(string message, Exception inner)
            : base(message, inner) { }
    }

    internal class PasswordStorage
    {
        // These constants may be changed without breaking existing hashes.
        private const int SaltBytes = 24;
        private const int HashBytes = 18;
        private const int Pbkdf2Iterations = 64000;

        // These constants define the encoding and may not be changed.
        private const int HashSections = 5;
        private const int HashAlgorithmIndex = 0;
        private const int IterationIndex = 1;
        private const int HashSizeIndex = 2;
        private const int SaltIndex = 3;
        private const int Pbkdf2Index = 4;

        public static string CreateHash(string password)
        {
            // Generate a random salt
            byte[] salt = new byte[SaltBytes];
            try
            {
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(salt);
            }
            catch (CryptographicException ex)
            {
                throw new CannotPerformOperationException(
                    "Random number generator not available.",
                    ex
                );
            }
            catch (ArgumentNullException ex)
            {
                throw new CannotPerformOperationException(
                    "Invalid argument given to random number generator.",
                    ex
                );
            }

            byte[] hash = Pbkdf2(password, salt, Pbkdf2Iterations, HashBytes);

            // format: algorithm:iterations:hashSize:salt:hash
            string parts = "sha1:" +
                           Pbkdf2Iterations +
                           ":" +
                           hash.Length +
                           ":" +
                           Convert.ToBase64String(salt) +
                           ":" +
                           Convert.ToBase64String(hash);
            return parts;
        }

        public static bool VerifyPassword(string password, string goodHash)
        {
            char[] delimiter = {':'};
            string[] split = goodHash.Split(delimiter);

            if (split.Length != HashSections)
            {
                throw new InvalidHashException(
                    "Fields are missing from the password hash."
                );
            }

            // We only support SHA1 with C#.
            if (split[HashAlgorithmIndex] != "sha1")
            {
                throw new CannotPerformOperationException(
                    "Unsupported hash type."
                );
            }

            int iterations;
            try
            {
                iterations = Int32.Parse(split[IterationIndex]);
            }
            catch (ArgumentNullException ex)
            {
                throw new CannotPerformOperationException(
                    "Invalid argument given to Int32.Parse",
                    ex
                );
            }
            catch (FormatException ex)
            {
                throw new InvalidHashException(
                    "Could not parse the iteration count as an integer.",
                    ex
                );
            }
            catch (OverflowException ex)
            {
                throw new InvalidHashException(
                    "The iteration count is too large to be represented.",
                    ex
                );
            }

            if (iterations < 1)
            {
                throw new InvalidHashException(
                    "Invalid number of iterations. Must be >= 1."
                );
            }

            byte[]? salt;
            try
            {
                salt = Convert.FromBase64String(split[SaltIndex]);
            }
            catch (ArgumentNullException ex)
            {
                throw new CannotPerformOperationException(
                    "Invalid argument given to Convert.FromBase64String",
                    ex
                );
            }
            catch (FormatException ex)
            {
                throw new InvalidHashException(
                    "Base64 decoding of salt failed.",
                    ex
                );
            }

            byte[]? hash = null;
            try
            {
                hash = Convert.FromBase64String(split[Pbkdf2Index]);
            }
            catch (ArgumentNullException ex)
            {
                throw new CannotPerformOperationException(
                    "Invalid argument given to Convert.FromBase64String",
                    ex
                );
            }
            catch (FormatException ex)
            {
                throw new InvalidHashException(
                    "Base64 decoding of pbkdf2 output failed.",
                    ex
                );
            }

            int storedHashSize = 0;
            try
            {
                storedHashSize = Int32.Parse(split[HashSizeIndex]);
            }
            catch (ArgumentNullException ex)
            {
                throw new CannotPerformOperationException(
                    "Invalid argument given to Int32.Parse",
                    ex
                );
            }
            catch (FormatException ex)
            {
                throw new InvalidHashException(
                    "Could not parse the hash size as an integer.",
                    ex
                );
            }
            catch (OverflowException ex)
            {
                throw new InvalidHashException(
                    "The hash size is too large to be represented.",
                    ex
                );
            }

            if (storedHashSize != hash.Length)
            {
                throw new InvalidHashException(
                    "Hash length doesn't match stored hash length."
                );
            }

            byte[] testHash = Pbkdf2(password, salt, iterations, hash.Length);
            return SlowEquals(hash, testHash);
        }

        private static bool SlowEquals(byte[] a, byte[] b)
        {
            uint diff = (uint) a.Length ^ (uint) b.Length;
            for (int i = 0; i < a.Length && i < b.Length; i++)
            {
                diff |= (uint) (a[i] ^ b[i]);
            }

            return diff == 0;
        }

        private static byte[] Pbkdf2(string password, byte[] salt, int iterations, int outputBytes)
        {
            using Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(password, salt);
            pbkdf2.IterationCount = iterations;
            return pbkdf2.GetBytes(outputBytes);
        }
    }
}

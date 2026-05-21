using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PlaniranjePutovanja.AuthService.Helpers.Passwords
{
    public sealed class PasswordHasher : IPasswordHasher
    {
        private readonly int _iterations;
        private readonly int _saltSize;
        private readonly int _hashSize;

        public PasswordHasher(int iterations = 100000, int saltSize = 16, int hashSize = 32)
        {
            _iterations = iterations;
            _saltSize = saltSize;
            _hashSize = hashSize;
        }
        public string Hash(string password)
        {
            var saltBytes = RandomNumberGenerator.GetBytes(_saltSize);
            var hashBytes = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                saltBytes,
                _iterations,
                HashAlgorithmName.SHA256,
                _hashSize);

            return $"{Convert.ToBase64String(saltBytes)}.{Convert.ToBase64String(hashBytes)}";
        }

        public bool Verify(string password, string passwordHash)
        {
            var parts = passwordHash.Split('.');
            if (parts.Length != 2)
            {
                return false;
            }

            var saltBytes = Convert.FromBase64String(parts[0]);
            var expectedHash = Convert.FromBase64String(parts[1]);

            var actualHash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                saltBytes,
                _iterations,
                HashAlgorithmName.SHA256,
                _hashSize);

            return CryptographicOperations.FixedTimeEquals(expectedHash, actualHash);
        }
    }
}

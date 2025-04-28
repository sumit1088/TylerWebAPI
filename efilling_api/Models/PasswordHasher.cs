using System.Security.Cryptography;
using System.Text;

namespace efilling_api.Models
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password, HashAlgorithmName algorithm)
        {
            using (var hashAlgorithm = HashAlgorithm.Create(algorithm.ToString()))
            {
                if (hashAlgorithm == null)
                    throw new ArgumentException("Invalid algorithm specified");

                var hashBytes = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hashBytes);
            }
        }

        public static bool VerifyPassword(string enteredPassword, string storedHash, HashAlgorithmName algorithm)
        {
            var enteredHash = HashPassword(enteredPassword, algorithm);
            return enteredHash == storedHash;
        }


    }
}
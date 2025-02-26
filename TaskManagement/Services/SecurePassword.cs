using System.Security.Cryptography;
using System.Text;

namespace TaskManagement.Services
{
    public static class SecurePassword
    {
        private const int saltSize = 16;
        private const int keySize = 64;
        private const int iterations = 350000;
        static HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        public static string Hash(string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(saltSize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                hashAlgorithm,
                keySize);
            byte[] hashWithSalt = new byte[saltSize + keySize];
            Buffer.BlockCopy(salt, 0, hashWithSalt, 0, saltSize);
            Buffer.BlockCopy(hash, 0, hashWithSalt, saltSize, keySize);

            return Convert.ToHexString(hashWithSalt);
        }

        public static bool Verify(string password, string hash)
        {
            byte[] hashWithSalt = Convert.FromHexString(hash);
            if (hashWithSalt.Length != saltSize + keySize)
            {
                return false;
            }

            byte[] salt = new byte[saltSize];
            byte[] storedHashBytes = new byte[keySize];

            Buffer.BlockCopy(hashWithSalt, 0, salt, 0, saltSize);
            Buffer.BlockCopy(hashWithSalt, saltSize, storedHashBytes, 0, keySize);

            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(Encoding.UTF8.GetBytes(password), salt, iterations, hashAlgorithm, keySize);
            return CryptographicOperations.FixedTimeEquals(hashToCompare, storedHashBytes);
        }

    }
}

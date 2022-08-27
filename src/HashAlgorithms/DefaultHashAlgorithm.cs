using System.Text;
using Operations;
using System.Security.Cryptography;

namespace HashAlgorithms
{
    internal class DefaultHashAlgorithm : IHashAlgorithm
    {
        public const int SALT_SIZE = 24; // size in bytes
        public const int HASH_SIZE = 24; // size in bytes
        public const int PBKDF2ITERATIONS = 2000; // number of pbkdf2 iterations
        public const int ITERATIONS = 5; // number of iterations

        protected byte[] GetPBKDF2Hash(in byte[] data, in byte[] key)
        {
            Rfc2898DeriveBytes pbkdf2 = new Rfc2898DeriveBytes(
                data,
                key,
                ITERATIONS
            );

            return pbkdf2.GetBytes(HASH_SIZE);
        }

        // The key is equal in length to the data
        protected byte[] HashBlock(ref byte[] data, in byte[] key)
        {
            BitOperations.SwapBitsByTemplate(ref data, key);

            BitOperations.XORBytes(ref data, key);

            return GetPBKDF2Hash(data, key);
        }

        public byte[] GetHash(in byte[] data, in byte[] salt)
        {
            byte[] currentData = (byte[])data.Clone();
            byte[] currentSalt = (byte[])data.Clone();
            for (int i = 0; i < ITERATIONS; i++)
            {
                currentData = HashBlock(ref currentData, currentSalt);
                currentSalt = GetPBKDF2Hash(currentData, currentSalt);
            }

            return currentData;
        }
    }
}

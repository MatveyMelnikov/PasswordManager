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
            /*BitPosition? swapPosition = null;

            for (int bytesIndex = 0; bytesIndex < key.Length; bytesIndex++)
            {
                for (byte bitsIndex = 0; bitsIndex < 8; bitsIndex++)
                {
                    if (BitOperations.GetBit(key[bytesIndex], bitsIndex))
                    {
                        if (swapPosition == null)
                        {
                            swapPosition = new BitPosition(bytesIndex, bitsIndex);
                        }
                        else
                        {
                            BitPosition currentPosition = new BitPosition(bytesIndex, bitsIndex);

                            BitOperations.SwapBits(
                                ref data, 
                                swapPosition ?? default(BitPosition), 
                                currentPosition
                            );
                            swapPosition = null;
                        }
                    }
                }
            }*/

            BitOperations.SwapBitsByTemplate(ref data, key);

            BitOperations.XORBytes(ref data, key);

            return GetPBKDF2Hash(data, key);
        }

        public byte[] GetHash(string data, string salt)
        {
            byte[] dataInBytes = Encoding.Default.GetBytes(data);
            byte[] saltInBytes = Encoding.Default.GetBytes(salt);
            for (int i = 0; i < ITERATIONS; i++)
            {
                dataInBytes = HashBlock(ref dataInBytes, saltInBytes);
                saltInBytes = GetPBKDF2Hash(dataInBytes, saltInBytes);
            }

            return dataInBytes;
        }
    }
}

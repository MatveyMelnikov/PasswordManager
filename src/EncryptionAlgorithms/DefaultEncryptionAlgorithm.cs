using Operations;

namespace EncryptionAlgorithms
{
    internal class DefaultEncryptionAlgorithm : IEncryptionAlgorithm
    {
        // Methods
        /// <summary>
        /// Byte array expansion. Used when the key is larger than the encrypted block
        /// </summary>
        protected byte[] ExpandByteArray(in byte[] data, in int length)
        {
            byte[] extendedData = new byte[length];
            for (int j = 0; j < extendedData.Length; j++)
            {
                extendedData[j] = data[j % data.Length];
            }

            return extendedData;
        }

        /// <summary>
        /// Returns a subsequence of elements as an array
        /// </summary>
        protected byte[] GetSubArray(in byte[] data, in int positionA, in int positionB)
        {
            byte[] result = new byte[positionB - positionA];
            Array.Copy(data, positionA, result, 0, positionB - positionA);
            return result;
        }

        /// <summary>
        /// Returns the position of the current encryption part as 
        /// the positions of the first and last elements
        /// </summary>
        protected (int, int) CalculatePositions(int numOfBlock, int dataLength, int keyLength)
        {
            int firstPosition = numOfBlock * keyLength;
            int lastPosition = firstPosition + keyLength;
            if (lastPosition > dataLength)
                lastPosition = dataLength;

            return (firstPosition, lastPosition);
        }

        public byte[] Encrypt(in byte[] data, in byte[] key)
        {
            byte[] result = (byte[])data.Clone();
            byte[] currentKey = (byte[])key.Clone();

            // The block is equal in length to the key size
            for (int i = 0; i < Math.Ceiling((float)result.Length / (float)currentKey.Length); i++)
            {
                (int firstPosition, int lastPosition) = CalculatePositions(
                    i,
                    result.Length, 
                    currentKey.Length
                );

                byte[] pastExtendedData = ExpandByteArray(
                    GetSubArray(result, firstPosition, lastPosition),
                    currentKey.Length
                );

                // Data encryption
                BitOperations.XORBytes(ref result, currentKey, firstPosition, lastPosition);

                byte[] newExtendedData = ExpandByteArray(
                    GetSubArray(result, firstPosition, lastPosition),
                    currentKey.Length
                );

                // Generation of the next key block
                BitOperations.SwapBitsByTemplate(ref currentKey, pastExtendedData);
                BitOperations.XORBytes(ref currentKey, newExtendedData);
            }

            return result;
        }

        public byte[] Decrypt(in byte[] data, in byte[] key)
        {
            byte[] result = (byte[])data.Clone();
            byte[] currentKey = (byte[])key.Clone();

            // The block is equal in length to the key size
            for (int i = 0; i < Math.Ceiling((float)result.Length / (float)currentKey.Length); i++)
            {
                (int firstPosition, int lastPosition) = CalculatePositions(
                    i, 
                    result.Length, 
                    currentKey.Length
                );

                byte[] pastExtendedData = ExpandByteArray(
                    GetSubArray(result, firstPosition, lastPosition),
                    currentKey.Length
                );

                // Data decryption
                BitOperations.XORBytes(ref result, currentKey, firstPosition, lastPosition);

                byte[] newExtendedData = ExpandByteArray(
                    GetSubArray(result, firstPosition, lastPosition),
                    currentKey.Length
                );

                // Generation of the next key block
                BitOperations.SwapBitsByTemplate(ref currentKey, newExtendedData);
                BitOperations.XORBytes(ref currentKey, pastExtendedData);
            }

            return result;
        }
    }
}

using System.Security.Cryptography;

namespace HashAlgorithms
{
    public class MyHashAlgorithm : IHashAlgorithm
    {
        public byte[] GetHash(in byte[] data, in byte[] salt)
        {
            byte[] currentData = (byte[])data.Clone();

            for (int i = 0; i < currentData.Length; i++)
                currentData[i] ^= salt[i];

            var md5Hash = MD5.Create();
            return md5Hash.ComputeHash(currentData);
        }
    }
}

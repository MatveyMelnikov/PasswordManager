namespace EncryptionAlgorithms
{
    public class MyEncryptionAlgorithm : IEncryptionAlgorithm
    {
        public byte[] Decrypt(in byte[] data, in byte[] key)
        {
            byte[] result = (byte[])data.Clone();
            byte[] currentKey = (byte[])key.Clone();

            for (int i = 0; i < result.Length; i++)
                result[i] = (byte)~result[i];

            return result;
        }

        public byte[] Encrypt(in byte[] data, in byte[] key)
        {
            byte[] result = (byte[])data.Clone();
            byte[] currentKey = (byte[])key.Clone();

            for (int i = 0; i < result.Length; i++)
                result[i] = (byte)~result[i];

            return result;
        }
    }
}

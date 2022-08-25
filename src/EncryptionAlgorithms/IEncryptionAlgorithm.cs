namespace EncryptionAlgorithms
{
    internal interface IEncryptionAlgorithm
    {
        public byte[] Encrypt(in byte[] data, in byte[] key);
        public byte[] Decrypt(in byte[] data, in byte[] key);
    }
}

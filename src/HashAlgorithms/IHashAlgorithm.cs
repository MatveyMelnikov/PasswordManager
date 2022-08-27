namespace HashAlgorithms
{
    internal interface IHashAlgorithm
    {
        public byte[] GetHash(in byte[] data, in byte[] salt);
    }
}

namespace HashAlgorithms
{
    internal interface IHashAlgorithm
    {
        public byte[] GetHash(string data, string salt);
    }
}

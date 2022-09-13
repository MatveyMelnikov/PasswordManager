namespace FileSystem
{
    public struct LoginData
    {
        public byte[] Hash;
        public byte[] Salt;

        public LoginData(byte[] hash, byte[] salt)
        {
            Hash = hash;
            Salt = salt;
        }
    }
}
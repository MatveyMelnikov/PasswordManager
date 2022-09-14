namespace FileSystem
{
    public struct LoginData
    {
        // Public fields
        public byte[] Hash;
        public byte[] Salt;

        // Methods
        public LoginData(byte[] hash, byte[] salt)
        {
            Hash = hash;
            Salt = salt;
        }
    }
}
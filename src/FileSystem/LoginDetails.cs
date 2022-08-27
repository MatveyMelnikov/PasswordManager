namespace FileSystem
{
    public struct LoginDetails
    {
        public LoginDetails(byte[] hash, byte[] salt)
        {
            Hash = hash;
            Salt = salt;
        }
        public byte[] Hash;
        public byte[] Salt;
    }
}
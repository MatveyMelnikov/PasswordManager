namespace EncryptionAlgorithms
{
    public class MyEncryptionPlugin : IEncryptionAlgorithm
    {
        public byte[] Decrypt(in byte[] data, in byte[] key)
        {
            Console.WriteLine("Plugin decrypt success");
            return new byte[0];
        }

        public byte[] Encrypt(in byte[] data, in byte[] key)
        {
            Console.WriteLine("Plugin encrypt success");
            return new byte[0];
        }
    }
}

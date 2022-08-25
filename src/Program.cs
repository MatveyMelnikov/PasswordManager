using System.Text;
using HashAlgorithms;
using EncryptionAlgorithms;
using Operations;
using System.Security.Cryptography;

class Program
{
    static byte[] GenerateSalt(int length)
    {
        return RandomNumberGenerator.GetBytes(length);
    }

    // First write the number of characters in the string
    static byte[] ReadStringInBytes(in BinaryReader reader)
    {
        int stringSize = reader.ReadInt32();
        return reader.ReadBytes(stringSize);
    }

    static void WriteStringInBytes(in BinaryWriter writer, in string data)
    {
        byte[] dataInBytes = Encoding.Default.GetBytes(data);
        writer.Write(dataInBytes.Length);
        writer.Write(dataInBytes);
    }

    static void Main(string[] args)
    {
        IEncryptionAlgorithm encryptionAlgorithm = new DefaultEncryptionAlgorithm();

        string data = "Any string to encrypt";
        byte[] key = { 65, 65, 66, 66 };
        byte[] salt = GenerateSalt(data.Length);

        // Encryption
        byte[] dataInBytes = Encoding.Default.GetBytes(data);
        BitOperations.XORBytes(ref dataInBytes, salt);

        byte[] encryptedData = encryptionAlgorithm.Encrypt(dataInBytes, key);

        Console.WriteLine("Encrypted data:");
        BitOperations.PrintBytes(encryptedData);

        // Decryption
        byte[] decryptedData = encryptionAlgorithm.Decrypt(encryptedData, key);
        BitOperations.XORBytes(ref decryptedData, salt);

        Console.WriteLine("Result:");
        Console.WriteLine(Encoding.Default.GetString(decryptedData));
    }
}
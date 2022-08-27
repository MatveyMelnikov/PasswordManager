using System.Text;
using System.Security.Cryptography;
using HashAlgorithms;
using EncryptionAlgorithms;
using FileSystem;
using Operations;

namespace Controllers
{
    internal class Controller : IController
    {
        protected FileManager? _fileManager = null;
        protected IHashAlgorithm? _hashAlgorithm = null;
        protected IEncryptionAlgorithm? _encryptionAlgorithm = null;
        protected byte[]? _key = null;

        public bool IsRegistered
        {
            get => !(_fileManager?.LoginDetails == null);
        }

        public void Init()
        {
            _fileManager = new("Data");
            _hashAlgorithm = new DefaultHashAlgorithm();
            _encryptionAlgorithm = new DefaultEncryptionAlgorithm();
        }

        public bool Enter(string firstKeyWord, string secondKeyWord)
        {
            if (!IsInit())
                throw new Exception("Controller did not initialize required fields");

            _key = Encoding.Default.GetBytes(firstKeyWord + secondKeyWord);

            if (!IsRegistered)
            {
                Register();
                return true;
            }

            LoginDetails? loginDetails = _fileManager!.LoginDetails;
            byte[] decryptedHash = _encryptionAlgorithm!.Decrypt(loginDetails!.Value.Hash, _key);
            byte[] hash = _hashAlgorithm!.GetHash(_key, loginDetails!.Value.Salt);

            if (Enumerable.SequenceEqual(hash, decryptedHash))
                return true;
            else
                return false;
        }

        protected void Register()
        {
            if (!IsInit() || _key == null)
                return;

            byte[] salt = GenerateSalt(_key!.Length);
            byte[] hash = _hashAlgorithm!.GetHash(_key, salt);
            byte[] encryptedKey = _encryptionAlgorithm!.Encrypt(hash, _key);

            _fileManager!.SaveLoginDetails(new LoginDetails(encryptedKey, salt));
        }

        public void Encrypt(
            string target,
            string firstField,
            string secondField
        )
        {
            if (!IsInit() || _key == null)
                return;

            byte[] firstFieldInBytes = Encoding.Default.GetBytes(firstField);
            byte[] secondFieldInBytes = Encoding.Default.GetBytes(secondField);
            byte[] firstSalt = GenerateSalt(firstFieldInBytes.Length);
            byte[] secondSalt = GenerateSalt(secondFieldInBytes.Length);

            BitOperations.XORBytes(ref firstFieldInBytes, firstSalt);
            byte[] firstEncryptedField = _encryptionAlgorithm!.Encrypt(firstFieldInBytes, _key);
            BitOperations.XORBytes(ref secondFieldInBytes, secondSalt);
            byte[] secondEncryptedField = _encryptionAlgorithm!.Encrypt(secondFieldInBytes, _key);


            _fileManager!.AddNote(
                new Note(target, firstEncryptedField, secondEncryptedField, firstSalt, secondSalt)
            );
        }

        public void DeleteNote(int position)
        {
            if (!IsInit())
                return;

            _fileManager!.DeleteNote(position);
        }

        public string[]? GetNotesTitles()
        {
            if (!IsInit())
                return null;

            string[] titles = new string[_fileManager!.Notes.Count];

            for (int i = 0; i < titles.Length; i++)
                titles[i] = _fileManager.Notes[i].target;

            return titles;
        }

        public string[]? GetNote(int position)
        {
            if (!IsInit() || _key == null || position < 0 || position >= _fileManager!.Notes.Count)
                return null;

            Note note = _fileManager.Notes[position];

            byte[] firstDecryptedField = _encryptionAlgorithm!.Decrypt(note.firstField, _key);
            BitOperations.XORBytes(ref firstDecryptedField, note.firstSalt);
            byte[] secondDecryptedField = _encryptionAlgorithm.Decrypt(note.secondField, _key);
            BitOperations.XORBytes(ref secondDecryptedField, note.secondSalt);

            string[] result = new string[3];
            result[0] = _fileManager.Notes[position].target;
            result[1] = Encoding.Default.GetString(firstDecryptedField);
            result[2] = Encoding.Default.GetString(secondDecryptedField);

            return result;
        }

        protected byte[] GenerateSalt(int length)
        {
            return RandomNumberGenerator.GetBytes(length);
        }

        bool IsInit()
        {
            if (_fileManager == null || _hashAlgorithm == null || _encryptionAlgorithm == null)
                return false;
            else
                return true;
        }

        public string? GetDataFileName()
        {
            if (!IsInit())
                return null;

            return _fileManager!.DataFileName;
        }

        public string[]? GetAllDataFiles()
        {
            if (!IsInit())
                return null;

            return _fileManager!.GetAllDataFiles();
        }

        public void ChangeDataFileName(string name)
        {
            if (!IsInit())
                return;

            _fileManager!.DataFileName = name;
        }
    }
}

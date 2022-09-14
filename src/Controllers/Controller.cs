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
        // Public fields
        public bool IsRegistered
        {
            get => !(_fileManager?.LoginDetails == null);
        }
        protected string currentPlugin = "default"; 
        public string CurrentPlugin 
        { 
            get => currentPlugin;
        }
        public string[]? NotesTitles 
        { 
            get
            {
                checkInit();

                string[] notes = new string[_fileManager!.Notes.Count];

                for (int i = 0; i < notes.Length; i++)
                    notes[i] = _fileManager.Notes[i].target;

                return notes;
            }
        }
        public string? DataFileName 
        { 
            get => _fileManager?.DataFileName;
        }
        public string[]? AllDataFilesTitles 
        { 
            get => _fileManager!.GetAllDataFiles(); 
        }
        public string[]? AllPluginsTitles 
        { 
            get => _pluginManager.GetAllPluginsTitles(); 
        }

        // Protected fields
        protected FileManager? _fileManager = null;
        protected PluginManager _pluginManager = new();
        protected IHashAlgorithm? _hashAlgorithm = null;
        protected IEncryptionAlgorithm? _encryptionAlgorithm = null;
        protected byte[]? _key = null;

        // Methods
        public void Init()
        {
            _fileManager = new("Data");

            SetPlugin(_fileManager.CurrentConfig.DefaultAlgorithm, false, false);
            _fileManager.DataFileName = _fileManager.CurrentConfig.DefaultDataFile;
        }

        public bool Enter(string firstKeyWord, string secondKeyWord)
        {
            checkInit();

            _key = Encoding.Default.GetBytes(firstKeyWord + secondKeyWord);

            if (!IsRegistered)
            {
                Register();
                return true;
            }

            LoginData? loginDetails = _fileManager!.LoginDetails;
            byte[] decryptedHash = _encryptionAlgorithm!.Decrypt(loginDetails!.Value.Hash, _key);
            byte[] hash = _hashAlgorithm!.GetHash(_key, loginDetails!.Value.Salt);

            if (Enumerable.SequenceEqual(hash, decryptedHash))
                return true;
            else
                return false;
        }

        protected void Register()
        {
            checkInit();
            if (_key == null)
                return;

            byte[] salt = GenerateSalt(_key!.Length);
            byte[] hash = _hashAlgorithm!.GetHash(_key, salt);
            byte[] encryptedKey = _encryptionAlgorithm!.Encrypt(hash, _key);

            _fileManager!.LoginDetails = new LoginData(encryptedKey, salt);
        }

        protected void Encrypt(
            string target,
            string firstField,
            string secondField,
            IEncryptionAlgorithm encryptionAlgorithm
        )
        {
            checkInit();
            if (_key == null)
                return;

            byte[] firstFieldInBytes = Encoding.Default.GetBytes(firstField);
            byte[] secondFieldInBytes = Encoding.Default.GetBytes(secondField);
            byte[] firstSalt = GenerateSalt(firstFieldInBytes.Length);
            byte[] secondSalt = GenerateSalt(secondFieldInBytes.Length);

            BitOperations.XORBytes(ref firstFieldInBytes, firstSalt);
            byte[] firstEncryptedField = encryptionAlgorithm!.Encrypt(firstFieldInBytes, _key);
            BitOperations.XORBytes(ref secondFieldInBytes, secondSalt);
            byte[] secondEncryptedField = encryptionAlgorithm!.Encrypt(secondFieldInBytes, _key);


            _fileManager!.AddNote(
                new Note(target, firstEncryptedField, secondEncryptedField, firstSalt, secondSalt)
            );
        }

        public void Encrypt(
            string target,
            string firstField,
            string secondField
        )
        {
            if (_encryptionAlgorithm == null)
                return;
            Encrypt(target, firstField, secondField, _encryptionAlgorithm);
        }

        public void DeleteNote(int position)
        {
            checkInit();

            _fileManager!.DeleteNote(position);
        }

        protected string[]? Decrypt(int position, IEncryptionAlgorithm encryptionAlgorithm)
        {
            checkInit();
            if (_key == null || position < 0 || position >= _fileManager!.Notes.Count)
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

        public string[]? GetNote(int position)
        {
            if (_encryptionAlgorithm == null)
                return null;

            return Decrypt(position, _encryptionAlgorithm);
        }

        protected byte[] GenerateSalt(int length)
        {
            return RandomNumberGenerator.GetBytes(length);
        }

        public void ChangeDataFileName(string name)
        {
            checkInit();

            _fileManager!.DataFileName = name;
        }

        public void ReEncryptAllData(
            in IEncryptionAlgorithm newEncryptionAlgorithm,
            in IHashAlgorithm newHashAlgorithm
        )
        {
            checkInit();

            string[][] notes = new string[_fileManager!.Notes.Count][];
            string[]? files = _fileManager.GetAllDataFiles();
            if (files == null)
                return;

            foreach (string fileName in files)
            {
                _fileManager.DataFileName = fileName;

                for (int i = 0; i < _fileManager.Notes.Count; i++)
                {
                    string[]? note = Decrypt(i, _encryptionAlgorithm!);
                    if (note == null)
                        continue;

                    notes[i] = note;
                }
                _fileManager.DeleteAllNotes();

                for (int i = 0; i < notes.Length; i++)
                    Encrypt(notes[i][0], notes[i][1], notes[i][2], newEncryptionAlgorithm);
            }
        }

        public void SetPlugin(
            string pluginTitle, 
            bool reEncryptData, 
            bool reEncryptLoginData
        )
        {
            IEncryptionAlgorithm? newEncryptionAlgorithm = null;
            IHashAlgorithm? newHashAlgorithm = null;

            if (pluginTitle == "default")
            {
                newEncryptionAlgorithm = new DefaultEncryptionAlgorithm();
                newHashAlgorithm = new DefaultHashAlgorithm();
                currentPlugin = "default";
            }
            else
            {
                List<IEncryptionAlgorithm>? encryptionAlgorithms =
                    _pluginManager.GetEncryptionAlgorithms(pluginTitle);
                List<IHashAlgorithm>? hashAlgorithms =
                    _pluginManager.GetHashAlgorithms(pluginTitle);

                if (encryptionAlgorithms != null && encryptionAlgorithms.Count > 0)
                    newEncryptionAlgorithm = encryptionAlgorithms.First();
                else
                    newEncryptionAlgorithm = new DefaultEncryptionAlgorithm();

                if (hashAlgorithms != null && hashAlgorithms.Count > 0)
                    newHashAlgorithm = hashAlgorithms.First();
                else
                    newHashAlgorithm = new DefaultHashAlgorithm();
                currentPlugin = pluginTitle;
            }

            // ReEncrypt login data
            if (reEncryptLoginData)
            {
                checkInit();

                byte[] salt = GenerateSalt(_key!.Length);
                byte[] hash = newHashAlgorithm!.GetHash(_key, salt);
                byte[] encryptedKey = newEncryptionAlgorithm!.Encrypt(hash, _key);

                _fileManager!.LoginDetails = new LoginData(encryptedKey, salt);
                _fileManager.CurrentConfig = new ConfigData(
                    _fileManager.CurrentConfig.DefaultDataFile,
                    pluginTitle
                );
            }
            if (reEncryptData)
            {
                ReEncryptAllData(newEncryptionAlgorithm, newHashAlgorithm);
            }

            _encryptionAlgorithm = newEncryptionAlgorithm;
            _hashAlgorithm = newHashAlgorithm;
        }

        void checkInit()
        {
            if (_fileManager == null || _hashAlgorithm == null || _encryptionAlgorithm == null)
                throw new Exception("Controller did not initialize required fields");
        }
    }
}

using System.Reflection;
using System.Text.Json;

namespace FileSystem
{
    internal class FileManager
    {
        protected string _dataFolderPath = String.Empty;
        protected string _loginDetailsPath = String.Empty;
        protected string _configPath = String.Empty;

        protected LoginData? _loginDetails = null;
        public LoginData? LoginDetails
        {
            get => _loginDetails;
            protected set => _loginDetails = value;
        }
        protected List<Note> _notes = new();
        public List<Note> Notes
        {
            get => _notes;
            protected set => _notes = value; 
        }
        protected string _dataFilePath = String.Empty;
        public string DataFileName
        {
            get => Path.GetFileName(_dataFilePath);
            set
            {
                _dataFilePath = Path.Combine(
                    _dataFolderPath,
                    Path.GetFileNameWithoutExtension(value) + ".dat"
                );
                CreateFile(_dataFilePath);
                ReadNotes();
            }
        }
        protected ConfigData _currentConfig;
        public ConfigData CurrentConfig { 
            get => _currentConfig;  
            set
            {
                _currentConfig = value;
                SaveConfig();
            }
        }

        public FileManager(string DataFileName)
        {
            // Create (open) a data folder
            string? location = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            if (location == null)
                throw new Exception("Executable location not received");

            _dataFolderPath = Path.Combine(location,"Data");

            if (!Directory.Exists(_dataFolderPath))
                Directory.CreateDirectory(_dataFolderPath);

            // Create (open) a login details file
            _loginDetailsPath = Path.Combine(_dataFolderPath, "LoginDetails.dat");
            CreateFile(_loginDetailsPath);

            ReadLoginDetails();

            // Create (open) a data file
            this.DataFileName = DataFileName;

            // Create (open) a config file
            _configPath = Path.Combine(location, "Config.dat");
            if (!File.Exists(_configPath))
            {
                using (File.Create(_configPath)) { }
                CurrentConfig = new("Data", "default");
            }
            else
            {
                ReadConfig();
            }
        }

        protected void CreateFile(string path)
        {
            if (!File.Exists(path))
                using (File.Create(path)) { }
        }

        protected void ReadLoginDetails()
        {
            using (BinaryReader reader = new BinaryReader(
                File.Open(_loginDetailsPath, FileMode.OpenOrCreate, FileAccess.Read)
            ))
            {
                if (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    byte[] hash = ReadBytesWithSize(reader);
                    byte[] salt = ReadBytesWithSize(reader);
                    _loginDetails = new(hash, salt);
                }
            }
        }

        public void SaveLoginDetails(LoginData loginDetails)
        {
            using (BinaryWriter writer = new BinaryWriter(
                File.Open(_loginDetailsPath, FileMode.Create, FileAccess.Write)
            ))
            {
                WriteBytesWithSize(writer, loginDetails.Hash);
                WriteBytesWithSize(writer, loginDetails.Salt);
            }
        }

        // Firstly write the number of characters in the string
        protected byte[] ReadBytesWithSize(in BinaryReader reader)
        {
            int stringSize = reader.ReadInt32();
            return reader.ReadBytes(stringSize);
        }

        protected void WriteBytesWithSize(in BinaryWriter writer, in byte[] data)
        {
            writer.Write(data.Length);
            writer.Write(data);
        }

        protected void ReadNotes()
        {
            _notes.Clear();
            using (BinaryReader reader = new BinaryReader(
                File.Open(_dataFilePath, FileMode.OpenOrCreate, FileAccess.Read)
            ))
            {
                while (reader.BaseStream.Position != reader.BaseStream.Length)
                {
                    string target = reader.ReadString();
                    byte[] firstField = ReadBytesWithSize(reader);
                    byte[] secondField = ReadBytesWithSize(reader);
                    byte[] firstSalt = ReadBytesWithSize(reader);
                    byte[] secondSalt = ReadBytesWithSize(reader);

                    _notes.Add(new Note(target, firstField, secondField, firstSalt, secondSalt));
                }
            }
        }

        public void SaveNotes()
        {
            using (BinaryWriter writer = new BinaryWriter(
                File.Open(_dataFilePath, FileMode.Create, FileAccess.Write)
            ))
            {
                foreach (Note note in _notes)
                {
                    writer.Write(note.target);
                    WriteBytesWithSize(writer, note.firstField);
                    WriteBytesWithSize(writer, note.secondField);
                    WriteBytesWithSize(writer, note.firstSalt);
                    WriteBytesWithSize(writer, note.secondSalt);
                }
            }
        }

        public void AddNote(Note note)
        {
            _notes.Add(note);
            SaveNotes();
        }

        public void AddNote(Note note, int position)
        {
            if (position < 0 || position >= _notes.Count)
                return;

            _notes.Insert(position, note);
            SaveNotes();
        }

        public void DeleteNote(int position)
        {
            if (position < 0 || position >= _notes.Count)
                return;

            _notes.RemoveAt(position);
            SaveNotes();
        }

        public void DeleteAllNotes()
        {
            _notes.Clear();
            SaveNotes();
        }

        public string[]? GetAllDataFiles()
        {
            // Get all data files in data folder
            string? location = Path.GetDirectoryName(Assembly.GetEntryAssembly()?.Location);
            if (location == null)
                return null;

            IEnumerable<string> dataFiles = Directory.GetFiles(_dataFolderPath, "*")
                .Where(path => Path.GetExtension(path) == ".dat" && 
                    Path.GetFileName(path) != "LoginDetails.dat")
                .Select(path => Path.GetFileName(path));

            return dataFiles.ToArray();
        }

        protected void ReadConfig()
        {
            using (StreamReader reader = new StreamReader(
                File.Open(_configPath, FileMode.OpenOrCreate, FileAccess.Read)
            ))
            {
                _currentConfig = JsonSerializer.Deserialize<ConfigData>(reader.ReadToEnd());
            }
        }

        public void SaveConfig()
        {
            using (StreamWriter writer = new StreamWriter(
                File.Open(_configPath, FileMode.Create, FileAccess.Write)
            ))
            {
                Console.WriteLine(JsonSerializer.Serialize(
                    _currentConfig,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    }
                ));

                writer.Write(JsonSerializer.Serialize(
                    _currentConfig,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true
                    }
                ));
            }
        }
    }
}

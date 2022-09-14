namespace Controllers
{
    internal interface IController
    {
        // Public fields
        public bool IsRegistered { get; }
        public string CurrentPlugin { get; }
        public string[]? NotesTitles { get; }
        public string? DataFileName { get; }
        public string[]? AllDataFilesTitles { get; }
        public string[]? AllPluginsTitles { get; }

        // Methods
        public void Init();
        public bool Enter(string firstKeyWord, string secondKeyWord);
        public void Encrypt(
            string target,
            string firstField,
            string secondField
        );
        public void DeleteNote(int position);
        public string[]? GetNote(int position);
        public void ChangeDataFileName(string name);
        public void SetPlugin(
            string pluginTitle,
            bool reEncryptData,
            bool reEncryptLoginData
        );
    }
}

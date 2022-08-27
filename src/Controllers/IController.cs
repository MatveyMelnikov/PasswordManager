namespace Controllers
{
    internal interface IController
    {
        public bool IsRegistered { get; }

        public void Init();
        public bool Enter(string firstKeyWord, string secondKeyWord);
        public void Encrypt(
            string target,
            string firstField,
            string secondField
        );
        public void DeleteNote(int position);
        public string[]? GetNotesTitles();
        public string[]? GetNote(int position);
        public string? GetDataFileName();
        public string[]? GetAllDataFiles();
        public void ChangeDataFileName(string name);
    }
}

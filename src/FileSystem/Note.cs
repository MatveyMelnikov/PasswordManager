namespace FileSystem
{
    public struct Note
    {
        // Public fields
        public string target;
        public byte[] firstField;
        public byte[] secondField;
        public byte[] firstSalt;
        public byte[] secondSalt;

        // Methods
        public Note(
            string target, 
            byte[] firstField, 
            byte[] secondField, 
            byte[] firstSalt,
            byte[] secondSalt
        )
        {
            this.target = target;
            this.firstField = firstField;
            this.secondField = secondField;
            this.firstSalt = firstSalt;
            this.secondSalt = secondSalt;
        }
    }
}

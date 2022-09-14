namespace FileSystem
{
    public struct ConfigData
    {
        // Public fields
        public string DefaultDataFile { get; set; }
        public string DefaultAlgorithm { get; set; }

        // Methods
        public ConfigData(string defaultDataFile, string defaultAlgorithm)
        {
            DefaultDataFile = defaultDataFile;
            DefaultAlgorithm = defaultAlgorithm;
        }
    }
}

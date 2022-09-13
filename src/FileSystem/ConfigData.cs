namespace FileSystem
{
    public struct ConfigData
    {
        public string DefaultDataFile { get; set; }
        public string DefaultAlgorithm { get; set; }

        public ConfigData(string defaultDataFile, string defaultAlgorithm)
        {
            DefaultDataFile = defaultDataFile;
            DefaultAlgorithm = defaultAlgorithm;
        }
    }
}

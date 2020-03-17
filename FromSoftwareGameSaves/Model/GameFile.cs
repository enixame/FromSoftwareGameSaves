namespace FromSoftwareGameSaves.Model
{
    public abstract class GameFile
    {
        protected GameFile(string rootDirectory, string fileSearchPattern)
        {
            RootDirectory = rootDirectory;
            FileSearchPattern = fileSearchPattern;
        }

        public string RootDirectory { get; }

        public string FileSearchPattern { get; }
    }
}
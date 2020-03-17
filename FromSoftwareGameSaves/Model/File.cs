namespace FromSoftwareGameSaves.Model
{
    public sealed class FromSoftwareFile : GameFile
    {
        public FromSoftwareFile(string rootDirectory, string fileSearchPattern, string fileName, bool isDirectory, string path) 
            : base(rootDirectory, fileSearchPattern)
        {
            FileName = fileName;
            IsDirectory = isDirectory;
            Path = path;
        }

        public string FileName { get; }

        public bool IsDirectory { get; }

        public string Path { get; internal set; }
    }
}

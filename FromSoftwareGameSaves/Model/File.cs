namespace FromSoftwareGameSaves.Model
{
    public sealed class File
    {
        public File(string fileName, bool isDirectory, string path)
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

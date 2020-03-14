using FromSoftwareModel.Interfaces;

namespace FromSoftwareModel
{
    public sealed class FromSoftwareFile : IGamePath
    {
        public string DirectoryName { get; }
        public string FileName { get; }

        public FromSoftwareFile(string directoryName, string fileName)
        {
            DirectoryName = directoryName;
            FileName = fileName;
        }
    }
}

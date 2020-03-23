namespace FromSoftwareStorage.Interface
{
    public interface ISaveGameFile
    {
        string Name { get; }
        string Directory { get; }
        string DefaultFileName { get; }
        string GameRootDirectory { get; }
        string FileSearchPattern { get; }
    }
}

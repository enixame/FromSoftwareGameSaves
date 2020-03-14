namespace FromSoftwareModel.Interfaces
{
    public interface ISaveGameFile
    {
        string GameDirectory { get; }
        string DefaultFileName { get; }
    }
}

using FromSoftwareModel.Interfaces;

namespace FromSoftwareModel
{
    public class SekiroGameDirectory : ISaveGameFile
    {
        public string GameDirectory => "Sekiro";
        public string DefaultFileName => "S0000.sl2";
        public string RootDirectory => FromSoftwareFileInfo.AppDataPath;
        public string FileSearchPattern => FromSoftwareFileInfo.FileSearchPattern;
    }
}

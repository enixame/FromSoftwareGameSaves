using FromSoftwareModel.Interfaces;

namespace FromSoftwareModel
{
    public class DarkSouls3GameDirectory : ISaveGameFile
    {
        public string GameDirectory => "DarkSoulsIII";
        public string DefaultFileName => "DS30000.sl2";
        public string RootDirectory => FromSoftwareFileInfo.AppDataPath;
        public string FileSearchPattern => FromSoftwareFileInfo.FileSearchPattern;

    }
}

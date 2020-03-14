using FromSoftwareModel.Interfaces;

namespace FromSoftwareModel
{
    public class SekiroGameDirectory : ISaveGameFile
    {
        public string GameDirectory => "Sekiro";
        public string DefaultFileName => "S0000.sl2";
    }
}

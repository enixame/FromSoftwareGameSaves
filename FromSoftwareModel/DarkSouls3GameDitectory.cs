using FromSoftwareModel.Interfaces;

namespace FromSoftwareModel
{
    public class DarkSouls3GameDitectory : ISaveGameFile
    {
        public string GameDirectory => "DarkSoulsIII";
        public string DefaultFileName => "DS30000.sl2";
    }
}

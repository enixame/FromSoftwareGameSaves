using FromSoftwareModel.Interfaces;

namespace FromSoftwareModel
{
    public static class FromSoftwareSaveFileRepository
    {
        public static ISaveGameFile[] FromSoftwareGameDirectories = {
            new DarkSouls3GameDitectory(), new SekiroGameDirectory(), 
        };
    }
}

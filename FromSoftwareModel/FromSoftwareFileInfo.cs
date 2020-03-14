using System;

namespace FromSoftwareModel
{
    public static class FromSoftwareFileInfo
    {
        public static readonly string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        public static readonly string FileSearchpattern = "*.sl2";
    }
}

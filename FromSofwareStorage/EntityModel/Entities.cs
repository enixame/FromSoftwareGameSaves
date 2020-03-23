using System;
using FromSoftwareStorage.Interface;

namespace FromSoftwareStorage.EntityModel
{
    public partial class Game : ISaveGameFile
    {
        private const Environment.SpecialFolder EmptySpecialFolder = (Environment.SpecialFolder) 999;

        public string GameRootDirectory
        {
            get
            {
                Folder folder = Folder;
                if (!string.IsNullOrEmpty(folder.FolderPath))
                    return folder.FolderPath;

                Environment.SpecialFolder specialFolder = GetSpecialFolder(folder.Name);
                if (specialFolder == EmptySpecialFolder)
                    return folder.FolderPath;
                return Environment.GetFolderPath(specialFolder);
            }
        }

        private static Environment.SpecialFolder GetSpecialFolder(string folderName)
        {
            return Enum.TryParse(folderName, true, out Environment.SpecialFolder enumFolder) 
                ? enumFolder 
                : EmptySpecialFolder;
        }
    }

    
}

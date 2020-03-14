using System.Collections.Generic;
using System.IO;
using System.Linq;
using FromSoftwareFileManager;
using FromSoftwareModel;
using File = FromSoftwareGameSaves.Model.File;

namespace FromSoftwareGameSaves.Repository
{
    public static class FileRepository
    {
        private static readonly List<File> Empty = new List<File>();

        public static List<File> LoadRootFiles()
        {
            var fromSoftwareSaveFiles = FromSoftwareSaveFileRepository.FromSoftwareGameDirectories;
            return fromSoftwareSaveFiles
                .Select(fromSoftwareSaveFile => new File(fromSoftwareSaveFile.GameDirectory, true, string.Empty))
                .Where(file => Directory.Exists(Path.Combine(FromSoftwareFileInfo.AppDataPath, file.FileName)))
                .ToList();
        }

        public static List<File> LoadChildren(File file)
        {
            if(!file.IsDirectory)
                return Empty;

            var filePath = Path.Combine(file.Path, file.FileName);

            var children = 
                FromSoftwareFileSearch.GetFiles(filePath, FromSoftwareFileInfo.FileSearchpattern)
                .Union(FromSoftwareFileSearch.GetDirectories(filePath))
                .Select(childrenFile => new File(Path.GetFileName(childrenFile), Directory.Exists(childrenFile), filePath))
                .ToList();

            return children;
        }
    }
}

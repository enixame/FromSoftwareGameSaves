using System.Collections.Generic;
using System.IO;
using System.Linq;
using FromSoftwareFileManager;
using FromSoftwareGameSaves.Model;
using FromSoftwareModel;

namespace FromSoftwareGameSaves.Repository
{
    public static class FileRepository
    {
        private static readonly List<FromSoftwareFile> Empty = new List<FromSoftwareFile>();

        public static List<FromSoftwareFile> LoadRootFiles()
        {
            var fromSoftwareSaveFiles = FromSoftwareSaveFileRepository.FromSoftwareGameDirectories;
            return fromSoftwareSaveFiles
                .Select(fromSoftwareSaveFile => new FromSoftwareFile(fromSoftwareSaveFile.RootDirectory, fromSoftwareSaveFile.FileSearchPattern, fromSoftwareSaveFile.GameDirectory, true, string.Empty))
                .Where(file => Directory.Exists(Path.Combine(file.RootDirectory, file.FileName)))
                .ToList();
        }

        public static List<FromSoftwareFile> LoadChildren(FromSoftwareFile fromSoftwareFile)
        {
            if(!fromSoftwareFile.IsDirectory)
                return Empty;

            var filePath = Path.Combine(fromSoftwareFile.Path, fromSoftwareFile.FileName);

            var children = 
                FromSoftwareFileSearch.GetFiles(filePath, fromSoftwareFile.FileSearchPattern, fromSoftwareFile.RootDirectory)
                .Union(FromSoftwareFileSearch.GetDirectories(filePath, fromSoftwareFile.RootDirectory))
                .Select(childrenFile => new FromSoftwareFile(fromSoftwareFile.RootDirectory, fromSoftwareFile.FileSearchPattern, Path.GetFileName(childrenFile), Directory.Exists(childrenFile), filePath))
                .ToList();

            return children;
        }
    }
}

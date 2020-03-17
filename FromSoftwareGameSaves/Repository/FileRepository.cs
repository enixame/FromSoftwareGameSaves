using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FromSoftwareFileManager;
using FromSoftwareGameSaves.Model;
using FromSoftwareModel;

namespace FromSoftwareGameSaves.Repository
{
    public static class FileRepository
    {
        private static readonly List<FromSoftwareFile> Empty = new List<FromSoftwareFile>();

        public static async Task<List<FromSoftwareFile>> LoadGemDirectoriesAsync()
        {
            var fromSoftwareGameDirectories = FromSoftwareSaveFileRepository.FromSoftwareGameDirectories;

            return await Task.Run(() =>
            {
                return 
                    fromSoftwareGameDirectories
                        .Select(fromSoftwareSaveFile => new FromSoftwareFile(fromSoftwareSaveFile.RootDirectory, fromSoftwareSaveFile.FileSearchPattern, fromSoftwareSaveFile.GameDirectory, true, string.Empty))
                        .Where(file => Directory.Exists(Path.Combine(file.RootDirectory, file.FileName)))
                        .ToList();
            });
        }

        public static async Task<List<FromSoftwareFile>> LoadChildrenAsync(FromSoftwareFile fromSoftwareFile)
        {
            if (!fromSoftwareFile.IsDirectory)
                return Empty;

            var filePath = Path.Combine(fromSoftwareFile.Path, fromSoftwareFile.FileName);

            return await Task.Run(() =>
            {
                return
                    FromSoftwareFileSearch.GetGameFiles(fromSoftwareFile.RootDirectory, filePath, fromSoftwareFile.FileSearchPattern)
                        .Union(FromSoftwareFileSearch.GetSubDirectories(fromSoftwareFile.RootDirectory, filePath))
                        .Select(childrenFile => new FromSoftwareFile(fromSoftwareFile.RootDirectory, fromSoftwareFile.FileSearchPattern, Path.GetFileName(childrenFile), Directory.Exists(childrenFile), filePath))
                        .ToList();
            });
        }
    }
}

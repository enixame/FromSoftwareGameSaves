using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FromSoftwareFileManager;
using FromSoftwareGameSaves.Model;
using FromSoftwareGameSaves.Utils;
using FromSoftwareStorage;
using FromSoftwareStorage.EntityModel;
using FromSoftwareStorage.Interface;

namespace FromSoftwareGameSaves.Repository
{
    public static class FileRepository
    {
        private static readonly List<FromSoftwareFile> Empty = new List<FromSoftwareFile>();

        public static async Task<List<FromSoftwareFile>> LoadGameDirectoriesAsync()
        {
            return await Task.Run(() =>
            {
                return
                    GetFromSoftwareGames()
                        .Select(fromSoftwareSaveFile => new FromSoftwareFile(fromSoftwareSaveFile.GameRootDirectory, fromSoftwareSaveFile.FileSearchPattern, fromSoftwareSaveFile.Directory, true, string.Empty) { GameName = fromSoftwareSaveFile.Name })
                        .Where(file => Directory.Exists(Path.Combine(file.RootDirectory, file.FileName)))
                        .ToList();
            });
        }

        private static IEnumerable<ISaveGameFile> GetFromSoftwareGames()
        {
            using (DataEntities dataEntities = Database.DatabaseProvider.GetEntities(ConnectionStrings.DataEntities))
                return dataEntities.Games.Include("Folder").ToList();
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

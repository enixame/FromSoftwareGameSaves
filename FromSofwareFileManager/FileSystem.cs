using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace FromSoftwareFileManager
{
    public static class FileSystem
    {
        #region Create directory

        public static bool CreateDirectory(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            if (Directory.Exists(path))
               throw new InvalidOperationException($"Directory '{Path.GetFileName(path)}' already exists.");

            var directoryInfo = Directory.CreateDirectory(path);
            return directoryInfo.Exists;
        }

        #endregion

        #region CopyAsync

        public static async Task<bool> CopyAsync(string sourcePath, string destPath, string fileExtension, bool isDirectory)
        {
            if (string.IsNullOrEmpty(sourcePath) || string.IsNullOrEmpty(destPath))
                return false;

            if (sourcePath.Equals(destPath))
                return false;

            return !isDirectory
                ? await CopyFileAsync(sourcePath, destPath) 
                : await CopyDirectoryAsync(sourcePath, destPath, fileExtension);
        }

        private static async Task<bool> CopyDirectoryAsync(string sourcePath, string destPath, string fileExtension)
        {
            if (Directory.Exists(destPath))
                throw new InvalidOperationException($"Directory '{Path.GetFileName(destPath)}' already exists.");
            await DirectoryCopyAsync(sourcePath, destPath, fileExtension);
            return true;
        }

        private static async Task DirectoryCopyAsync(string sourcePath, string destPath, string fileExtension)
        {
            // create subDirectories
            var subDirectories = Directory.GetDirectories(sourcePath, "*.*", SearchOption.AllDirectories);

            if (subDirectories.Any())
            {
                foreach (var subDirectory in subDirectories)
                    Directory.CreateDirectory(subDirectory.Replace(sourcePath, destPath));
            }
            else
            {
                // create destination path.
                Directory.CreateDirectory(destPath);
            }
            
            // create files
            foreach (var file in Directory.GetFiles(sourcePath, fileExtension, SearchOption.AllDirectories))
                await CopyFileAsync(file, file.Replace(sourcePath, destPath));
        }

        private static async Task<bool> CopyFileAsync(string sourcePath, string destPath)
        {
            if (File.Exists(destPath))
                throw new InvalidOperationException($"File '{Path.GetFileName(destPath)}' already exists.");

            await CopyOrMoveFileAsync(sourcePath, destPath, false);

            return true;
        }

        #endregion

        #region delete

        public static bool Delete(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;

            var fileExists = File.Exists(path);
            var directoryExists = Directory.Exists(path);

            if (!fileExists && !directoryExists)
                throw new InvalidOperationException($"File or Directory '{Path.GetFileName(path)}' does not exist.");

            return fileExists ? RemoveFile(path) : RemoveDirectory(path);
        }

        private static bool RemoveFile(string path)
        {
            if (!File.Exists(path))
                throw new InvalidOperationException($"File '{Path.GetFileName(path)}' does not exist.");

            File.Delete(path);
            return true;
        }

        private static bool RemoveDirectory(string path)
        {
            if (!Directory.Exists(path))
                throw new InvalidOperationException($"Directory '{Path.GetFileName(path)}' does not  exists.");

            Directory.Delete(path, true);
            return true;
        }

        #endregion

        #region async copy/move

        private static async Task CopyOrMoveFileAsync(string sourcePath, string destinationPath, bool move)
        {
            using (Stream source = File.Open(sourcePath, FileMode.Open))
            {
                using (Stream destination = File.Create(destinationPath))
                {
                    await source.CopyToAsync(destination);
                }
            }

            if (move)
                File.Delete(sourcePath);
        }

        #endregion

        #region RenameAsync

        public static async Task<bool> RenameAsync(string oldPath, string newPath)
        {
            if (string.IsNullOrEmpty(oldPath) || string.IsNullOrEmpty(newPath))
                return false;

            if (oldPath.Equals(newPath))
                return false;

            var fileExists = File.Exists(oldPath);
            var directoryExists = Directory.Exists(oldPath);

            if(!fileExists && !directoryExists)
                throw new InvalidOperationException($"File or Directory '{Path.GetFileName(newPath)}' does not exist.");

            return fileExists ? await RenameFileAsync(oldPath, newPath) : RenameDirectory(oldPath, newPath);
        }

        private static async Task<bool> RenameFileAsync(string oldPath, string newPath)
        {
            if (File.Exists(newPath))
                throw new InvalidOperationException($"File '{Path.GetFileName(newPath)}' already exists.");

            await CopyOrMoveFileAsync(oldPath, newPath, true);
          
            return true;
        }

        private static bool RenameDirectory(string oldPath, string newPath)
        {
            if (Directory.Exists(newPath))
                throw new InvalidOperationException($"Directory '{Path.GetFileName(newPath)}' already exists.");

            Directory.Move(oldPath, newPath);
            return true;
        }

        #endregion
    }
}

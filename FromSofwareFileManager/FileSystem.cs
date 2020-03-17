using System;
using System.IO;
using System.Linq;

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

        #region Copy

        public static bool Copy(string sourcePath, string destPath, string fileExtension, bool isDirectory)
        {
            if (string.IsNullOrEmpty(sourcePath) || string.IsNullOrEmpty(destPath))
                return false;

            if (sourcePath.Equals(destPath))
                return false;

            return !isDirectory ? CopyFile(sourcePath, destPath) : CopyDirectory(sourcePath, destPath, fileExtension);
        }

        private static bool CopyDirectory(string sourcePath, string destPath, string fileExtension)
        {
            if (Directory.Exists(destPath))
                throw new InvalidOperationException($"Directory '{Path.GetFileName(destPath)}' already exists.");
            DirectoryCopy(sourcePath, destPath, fileExtension);
            return true;
        }

        private static void DirectoryCopy(string sourcePath, string destPath, string fileExtension)
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
                CopyFile(file, file.Replace(sourcePath, destPath));
        }

        private static bool CopyFile(string sourcePath, string destPath)
        {
            if (File.Exists(destPath))
                throw new InvalidOperationException($"File '{Path.GetFileName(destPath)}' already exists.");
            File.Copy(sourcePath, destPath, true);
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

        #region Rename

        public static bool Rename(string oldPath, string newPath)
        {
            if (string.IsNullOrEmpty(oldPath) || string.IsNullOrEmpty(newPath))
                return false;

            if (oldPath.Equals(newPath))
                return false;

            var fileExists = File.Exists(oldPath);
            var directoryExists = Directory.Exists(oldPath);

            if(!fileExists && !directoryExists)
                throw new InvalidOperationException($"File or Directory '{Path.GetFileName(newPath)}' does not exist.");

            return fileExists ? RenameFile(oldPath, newPath) : RenameDirectory(oldPath, newPath);
        }

        private static bool RenameFile(string oldPath, string newPath)
        {
            if (File.Exists(newPath))
                throw new InvalidOperationException($"File '{Path.GetFileName(newPath)}' already exists.");

            File.Move(oldPath, newPath);
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

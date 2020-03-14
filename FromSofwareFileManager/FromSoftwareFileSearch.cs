using System;
using System.IO;
using FromSoftwareModel;

namespace FromSoftwareFileManager
{
    public static class FromSoftwareFileSearch
    {
        
        private static readonly string[] Empty = new string[0];

        public static string[] GetFiles(string gamePath, string fileSearchpattern)
        {
            if(string.IsNullOrEmpty(fileSearchpattern) || !fileSearchpattern.Equals(FromSoftwareFileInfo.FileSearchpattern))
                return Empty;

            SearchOption searchOption;
            GetSearchOption(gamePath, out searchOption);

            string fullPath = FromSoftwareFileInfo.AppDataPath;
            if(!string.IsNullOrEmpty(gamePath))
                fullPath = Path.Combine(FromSoftwareFileInfo.AppDataPath, gamePath);
            return Directory.GetFiles(fullPath, fileSearchpattern, searchOption);
        }

        public static string[] GetDirectories(string gamePath)
        {
            SearchOption searchOption;
            GetSearchOption(gamePath, out searchOption);

            string fullPath = FromSoftwareFileInfo.AppDataPath;
            if (!string.IsNullOrEmpty(gamePath))
                fullPath = Path.Combine(FromSoftwareFileInfo.AppDataPath, gamePath);
           
            return Directory.GetDirectories(fullPath, "*.*", searchOption);
        }

        private static void GetSearchOption(string gamePath, out SearchOption searchOption)
        {
            searchOption = SearchOption.TopDirectoryOnly;

            if (string.IsNullOrEmpty(gamePath) || gamePath.Trim().Equals(string.Empty, StringComparison.Ordinal))
                searchOption = SearchOption.AllDirectories;
        }
    }
}

using System;
using System.IO;

namespace FromSoftwareFileManager
{
    public static class FromSoftwareFileSearch
    {
        
        private static readonly string[] Empty = new string[0];

        public static string[] GetFiles(string gamePath, string fileSearchPattern, string rootPath)
        {
            if(string.IsNullOrEmpty(fileSearchPattern))
                return Empty;

            GetSearchOption(gamePath, out SearchOption searchOption);

            string fullPath = rootPath;
            if(!string.IsNullOrEmpty(gamePath))
                fullPath = Path.Combine(rootPath, gamePath);
            return Directory.GetFiles(fullPath, fileSearchPattern, searchOption);
        }

        public static string[] GetDirectories(string gamePath, string rootPath)
        {
            GetSearchOption(gamePath, out SearchOption searchOption);

            string fullPath = rootPath;
            if (!string.IsNullOrEmpty(gamePath))
                fullPath = Path.Combine(rootPath, gamePath);
           
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

using System.IO;

namespace FromSoftwareFileManager
{
    public static class FromSoftwareFileSearch
    {
        
        private static readonly string[] Empty = new string[0];

        /// <summary>
        /// Gets game files from a directory using a search pattern.
        /// </summary>
        /// <param name="rootPath">Root path that contains all games path.</param>
        /// <param name="gamePath">Game path</param>
        /// <param name="fileSearchPattern">Files type you are looking for using a search pattern (e.g. *.sl2)</param>
        /// <returns>Game save files. Returns an empty array if fileSearchPattern is null or empty.</returns>
        public static string[] GetGameFiles(string rootPath, string gamePath, string fileSearchPattern)
        {
            if(string.IsNullOrEmpty(fileSearchPattern) || string.IsNullOrWhiteSpace(fileSearchPattern))
                return Empty;

            GetSearchOption(gamePath, out SearchOption searchOption);

            string fullPath = rootPath;
            if(!string.IsNullOrEmpty(gamePath))
                fullPath = Path.Combine(rootPath, gamePath);

            return Directory.GetFiles(fullPath, fileSearchPattern, searchOption);
        }

        /// <summary>
        /// Gets all sub-directories from a root Path.
        /// </summary>
        /// <param name="rootPath">Root path that contains all games path</param>
        /// <param name="gamePath">Game path</param>
        /// <returns></returns>
        public static string[] GetSubDirectories(string rootPath, string gamePath)
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

            if (string.IsNullOrEmpty(gamePath) || string.IsNullOrWhiteSpace(gamePath))
                searchOption = SearchOption.AllDirectories;
        }
    }
}

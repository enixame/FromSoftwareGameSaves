using System;
using System.IO;
using FromSoftwareFileManager;
using FromSoftwareModel;
using NUnit.Framework;

namespace FromSoftwareGameSaves.Test
{
    [TestFixture]
    public class FromSoftwareGamePathTests
    {
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("     ")]
        [TestCase(null)]
        public void FromSoftwareGamePathGetFilesTestsThrowsUnauthorized(string gamePath)
        {
            Assert.Throws<UnauthorizedAccessException>(() => FromSoftwareFileSearch.GetDirectories(gamePath, FromSoftwareFileInfo.AppDataPath));   
        }

        [Test]
        public void FromSoftwareGamePathGetFilesTestsThrowsNotFound()
        {
            Assert.Throws<DirectoryNotFoundException>(() => FromSoftwareFileSearch.GetFiles("toto", FromSoftwareFileInfo.FileSearchPattern, FromSoftwareFileInfo.AppDataPath));
        }

        [TestCase("DarkSoulsIII")]
        [TestCase("Sekiro")]
        public void FromSoftwareGamePathGetFilesTestsDoesNotThrowException(string gamePath)
        {
            string[] files = FromSoftwareFileSearch.GetFiles(gamePath, FromSoftwareFileInfo.FileSearchPattern, FromSoftwareFileInfo.AppDataPath);
            Assert.IsNotNull(files);
            Assert.IsTrue(files.Length == 0);
        }

        [TestCase("DarkSoulsIII")]
        [TestCase("Sekiro")]
        public void FromSoftwareGamePathGetDirectoriesTestsDoesNotThrowException(string gamePath)
        {
            string[] files = FromSoftwareFileSearch.GetDirectories(gamePath, FromSoftwareFileInfo.AppDataPath);
            Assert.IsNotNull(files);
            Assert.IsTrue(files.Length > 0);
            Console.Out.WriteLine($"files length: {files.Length}");
            foreach (string file in files)
            {
                Console.Out.WriteLine($"fromSoftwareFile: {file}");
            }
        }
    }
}

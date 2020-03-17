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
        private const string TestDirectory = "Test";
        private string _rootDirectory;

        [SetUp]
        public void Setup()
        {
            string directoryName = Path.GetDirectoryName(new Uri(typeof(FromSoftwareGamePathTests).Assembly.CodeBase).LocalPath);
            _rootDirectory = Path.Combine(directoryName ?? throw new InvalidOperationException(), TestDirectory);
        }

        [TestCase("")]
        [TestCase(" ")]
        [TestCase("     ")]
        [TestCase(null)]
        public void FromSoftwareGamePathGetFilesTestsThrowsUnauthorized(string gamePath)
        {
            Assert.Throws<UnauthorizedAccessException>(() => FromSoftwareFileSearch.GetSubDirectories(FromSoftwareFileInfo.AppDataPath, gamePath));   
        }

        [Test]
        public void FromSoftwareGamePathGetFilesTestsThrowsNotFound()
        {
            Assert.Throws<DirectoryNotFoundException>(() => FromSoftwareFileSearch.GetGameFiles(_rootDirectory, "toto", FromSoftwareFileInfo.FileSearchPattern));
        }

        [TestCase("Game1")]
        [TestCase("Game2")]
        public void FromSoftwareGamePathGetGameFileDoesNotThrowException(string gamePath)
        {
            string[] files = FromSoftwareFileSearch.GetGameFiles(_rootDirectory, gamePath, FromSoftwareFileInfo.FileSearchPattern);
            Assert.IsNotNull(files);
            Assert.IsTrue(files.Length == 0);
        }

        [TestCase("Game1")]
        [TestCase("Game2")]
        public void FromSoftwareGamePathGetSubDirectoriesDoesNotThrowException(string gamePath)
        {
            string[] files = FromSoftwareFileSearch.GetSubDirectories(_rootDirectory, gamePath);
            Assert.IsNotNull(files);
            Assert.IsTrue(files.Length > 0);
        }
    }
}

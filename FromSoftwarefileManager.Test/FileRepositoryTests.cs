using System;
using System.IO;
using System.Threading.Tasks;
using FromSoftwareGameSaves.Model;
using FromSoftwareGameSaves.Repository;
using NUnit.Framework;

namespace FromSoftwareGameSaves.Test
{
    [TestFixture]
    public class FileRepositoryTests
    {
        private const string TestDirectory = "Test";
        private string _rootDirectory;

        [SetUp]
        public void Setup()
        {
            string directoryName = Path.GetDirectoryName(new Uri(typeof(FileRepositoryTests).Assembly.CodeBase).LocalPath);
            _rootDirectory = Path.Combine(directoryName ?? throw new InvalidOperationException(), TestDirectory);
        }

        [Test]
        public async Task CheckGame1FilesAndDirCountTest()
        {
            var game1 = new FromSoftwareFile(_rootDirectory, "*.sl2", "Game1", true, string.Empty);
            var actualCount = await GetTree(game1);
            Assert.That(9, Is.EqualTo(actualCount));
        }

        [Test]
        public async Task CheckGame2FilesAndDirCountTest()
        {
            var game2 = new FromSoftwareFile(_rootDirectory, "*.sl2", "Game2", true, string.Empty);
            var actualCount = await GetTree(game2);
            Assert.That(7, Is.EqualTo(actualCount));
        }

        private static async Task<int> GetTree(FromSoftwareFile fromSoftwareFile)
        {
            int childrenCount = 0;
            var children = await FileRepository.LoadChildrenAsync(fromSoftwareFile);
            foreach (var child in children)
            {
                childrenCount++;
               int subChildrenCount = await GetTree(child);
               childrenCount += subChildrenCount;
            }

            return childrenCount;
        }
    }
}

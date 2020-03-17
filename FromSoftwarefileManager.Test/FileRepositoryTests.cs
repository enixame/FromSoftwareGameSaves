using System;
using System.IO;
using System.Threading.Tasks;
using FromSoftwareGameSaves.Model;
using FromSoftwareGameSaves.Repository;
using FromSoftwareModel;
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
            var game1 = new FromSoftwareFile(_rootDirectory, FromSoftwareFileInfo.FileSearchPattern, "Game1", true, string.Empty);
            var actualCount = await GetTree(game1, 0);
            Assert.That(9, Is.EqualTo(actualCount));
        }

        [Test]
        public async Task CheckGame2FilesAndDirCountTest()
        {
            var game2 = new FromSoftwareFile(_rootDirectory, FromSoftwareFileInfo.FileSearchPattern,"Game2", true, string.Empty);
            var actualCount = await GetTree(game2, 0);
            Assert.That(7, Is.EqualTo(actualCount));
        }

        private static async Task<int> GetTree(FromSoftwareFile fromSoftwareFile, int level)
        {
            int childrenCount = 0;
            var children = await FileRepository.LoadChildrenAsync(fromSoftwareFile);
            foreach (var child in children)
            {
                for (int i = 0; i < level; i++)
                   Console.Write(@" ");
                Console.Out.WriteLine($"{child.Path}\\{child.FileName}");
                childrenCount++;
               int subChildrenCount = await GetTree(child, level + 1);
               childrenCount += subChildrenCount;
            }

            return childrenCount;
        }
    }
}

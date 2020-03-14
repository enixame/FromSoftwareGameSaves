using System;
using FromSoftwareGameSaves.Model;
using FromSoftwareGameSaves.Repository;
using NUnit.Framework;

namespace FromSoftwareGameSaves.Test
{
    [TestFixture]
    public class FileRepositoryTests
    {
        
        [Test]
        public void LoadDarkSouls3Tree()
        {
            var ds3 = new File("DarkSoulsIII", true, string.Empty);
            GetTree(ds3, 0);
        }

        [Test]
        public void LoadDarkSekiroTree()
        {
            var sekiro = new File("Sekiro", true, string.Empty);
            GetTree(sekiro, 0);
        }

        private void GetTree(File file, int level)
        {
            var children = FileRepository.LoadChildren(file);
            foreach (var child in children)
            {
                for (int i = 0; i < level; i++)
                   Console.Write(@" ");
                Console.Out.WriteLine($"{child.Path}\\{child.FileName}");
                GetTree(child, level + 1);
            }
        }
    }
}

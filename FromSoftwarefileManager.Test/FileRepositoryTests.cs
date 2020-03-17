using System;
using FromSoftwareGameSaves.Model;
using FromSoftwareGameSaves.Repository;
using FromSoftwareModel;
using NUnit.Framework;

namespace FromSoftwareGameSaves.Test
{
    [TestFixture]
    public class FileRepositoryTests
    {
        
        [Test]
        public void LoadDarkSouls3Tree()
        {
            var ds3 = new FromSoftwareFile(FromSoftwareFileInfo.AppDataPath, FromSoftwareFileInfo.FileSearchPattern, "DarkSoulsIII", true, string.Empty);
            GetTree(ds3, 0);
        }

        [Test]
        public void LoadDarkSekiroTree()
        {
            var sekiro = new FromSoftwareFile(FromSoftwareFileInfo.AppDataPath, FromSoftwareFileInfo.FileSearchPattern,"Sekiro", true, string.Empty);
            GetTree(sekiro, 0);
        }

        private void GetTree(FromSoftwareFile fromSoftwareFile, int level)
        {
            var children = FileRepository.LoadChildren(fromSoftwareFile);
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

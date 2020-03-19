using System;
using System.IO;
using System.Reflection;
using FromSoftwareStorage;
using NUnit.Framework;

namespace FromSoftwareGameSaves.Test
{
    [TestFixture]
    public class RsaCryptoServiceTests
    {
        private const string TestDirectory = "Stockage";
        private const string FileName = "test.xml";
        private static readonly string RootPath = Path.GetDirectoryName(new Uri(typeof(RsaCryptoServiceTests).Assembly.CodeBase).LocalPath);

        private static readonly string FullPath = Path.Combine(RootPath, TestDirectory, FileName);

        private RsaCryptoService _rsaCryptoService;

        [SetUp]
        public void SetUp()
        {
            _rsaCryptoService = new RsaCryptoService();

            string path = Path.Combine(RootPath, TestDirectory);    
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if(File.Exists(FullPath))
                File.Delete(FullPath);
        }

        [TearDown]
        public void TearDown()
        {
            _rsaCryptoService?.Dispose();
        }

        [Test]
        public void GeneratePrivateKetTest()
        {
            RsaCryptoService.GeneratePrivateKey(FullPath);

            Assert.That(File.Exists(FullPath), Is.True);
            Console.Out.WriteLine(File.ReadAllText(FullPath));
        }

        [Test]
        public void EncryptDecryptDataTest()
        {
            RsaCryptoService.GeneratePrivateKey(FullPath);

            string data = "123456789";
            string encryptData = _rsaCryptoService.EncryptData(FullPath, data);

            Console.Out.WriteLine("Password: " + encryptData);

            string decryptData = _rsaCryptoService.DecryptData(FullPath, encryptData);

            Assert.That(data, Is.EqualTo(decryptData));
        }
    }
}

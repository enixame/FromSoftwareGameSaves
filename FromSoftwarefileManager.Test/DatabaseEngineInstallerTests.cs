using System.IO;
using System.Threading.Tasks;
using FromSoftwareStorage;
using NUnit.Framework;

namespace FromSoftwareGameSaves.Test
{
    [TestFixture]
    public class DatabaseEngineInstallerTests
    {
        private const string DefaultPassword = "123456789";
        private DatabaseEngineInstaller _engine;

        [SetUp]
        public void Setup()
        {
            _engine = new DatabaseEngineInstaller();

            if (_engine.DataBaseExists)
                File.Delete(_engine.DatabaseFileName);
        }

        [Test]
        public void TearDown()
        {
            if (_engine.HasPrivateKey)
                File.Delete(_engine.RsaPrivateKeyFileName);

            if (_engine.HasPublicKey)
                File.Delete(_engine.RsaPublicKeyFileName);
        }

        [Test]
        public async Task InstallDatabaseWithoutPasswordTest()
        {
           bool installSucceeded = await _engine.InstallDatabaseAsync();
           Assert.That(installSucceeded, Is.True);
           Assert.That(File.Exists(_engine.DatabaseFileName), Is.True);
        }

        [Test]
        public async Task InstallDatabaseWithPasswordTest()
        {
            bool installSucceeded = await _engine.InstallDatabaseAsync(DefaultPassword);
            Assert.That(installSucceeded, Is.True);
            Assert.That(File.Exists(_engine.DatabaseFileName), Is.True);
            Assert.That(File.Exists(_engine.RsaPrivateKeyFileName), Is.True);
            Assert.That(File.Exists(_engine.RsaPublicKeyFileName), Is.True);
        }
    }
}

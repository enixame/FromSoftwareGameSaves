using System.IO;
using System.Threading.Tasks;
using FromSoftwareStorage;
using NUnit.Framework;

namespace FromSoftwareGameSaves.Test
{
    [TestFixture]
    public class DatabaseEngineProviderTests
    {
        private const string DefaultPassword = "123456789";
        private DatabaseEngineProvider _engine;

        [SetUp]
        public void Setup()
        {
            _engine = new DatabaseEngineProvider();

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
        public void CreateDataWithoutPasswordTest()
        {
            _engine.CreateDatabase();
            Assert.That(File.Exists(_engine.DatabaseFileName), Is.True);
        }

        [Test]
        public void CreateDataWithPasswordTest()
        {
            _engine.CreateDatabase(DefaultPassword);
            Assert.That(File.Exists(_engine.DatabaseFileName), Is.True);
            Assert.That(File.Exists(_engine.RsaPublicKeyFileName), Is.True);
            Assert.That(File.Exists(_engine.RsaPrivateKeyFileName), Is.True);
        }

        [Test]
        public async Task InstallDatabaseWithoutPasswordTest()
        {
           bool installSucceeded = await _engine.InstallDatabaseAsync();
           Assert.That(installSucceeded, Is.True);
           Assert.That(File.Exists(_engine.DatabaseFileName), Is.True);
        }
    }
}

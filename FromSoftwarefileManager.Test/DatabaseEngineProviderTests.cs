using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using FromSoftwareStorage;
using FromSoftwareStorage.EntityModel;
using NUnit.Framework;
using Database = FromSoftwareStorage.Database;

namespace FromSoftwareGameSaves.Test
{
    [TestFixture]
    public class DatabaseEngineProviderTests
    {
        private const string ConnectionStringName = "DataEntities";
        private DatabaseEngineInstaller _installer;
        private DatabaseEngineProvider _provider;

        [SetUp]
        public async Task Setup()
        {
            _installer = new DatabaseEngineInstaller();
            _provider = new DatabaseEngineProvider();

            Uninstall();

            await _installer.InstallDatabaseAsync();
        }

        private void Uninstall()
        {
            if (_provider.HasDatabasePassword)
            {
                File.Delete(_installer.RsaPrivateKeyFileName);
                File.Delete(_installer.RsaPublicKeyFileName);
            }

            if (_provider.IsDatabaseInstalled)
            {
                File.Delete(_installer.DatabaseFileName);
            }
        }

        [Test]
        public async Task InsertNewGameTest()
        {
            IDatabaseProvider databaseProvider = Database.DatabaseProvider;
            using (DataEntities dataEntities = databaseProvider.GetEntities(ConnectionStringName))
            {
                DbSet<Game> dataEntitiesGames = dataEntities.Games;

                Game newGame = dataEntitiesGames.Create();
                newGame.Name = "Test";
                newGame.ChangeDate = DateTime.Now;
                newGame.DefaultFileName = "Test.txt";
                newGame.Directory = "Test";
                newGame.FileSearchPattern = "*.txt";
                newGame.ReadOnly = false;
                newGame.Folder = new Folder() { Name = "Test", FolderPath = @"C:\", ReadOnly = false };
                newGame.Image = new Image() { ImageFile = await GetBinaryResourceAsync("FromSoftwareGameSaves.Test.Test.Test.jpg") };
                dataEntitiesGames.Add(newGame);
                dataEntities.SaveChanges();

                IQueryable<Game> query = from game in dataEntitiesGames
                    where game.Name.Equals("Test")
                    select game;
                var gameWihTestName = query.FirstOrDefault();

                Assert.That(gameWihTestName, Is.EqualTo(newGame));
            }
   
        }

        public static async Task<byte[]> GetBinaryResourceAsync(string resourceName)
        {
            Assembly assembly = typeof(DatabaseEngineProviderTests).Assembly;
            using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (resourceStream == null) return new byte[0];
                byte[] bufferBytes = new byte[resourceStream.Length];
                await resourceStream.ReadAsync(bufferBytes, 0, bufferBytes.Length);

                return bufferBytes;
            }
        }
    }
}

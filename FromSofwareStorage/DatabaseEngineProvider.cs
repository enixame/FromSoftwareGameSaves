using System;
using System.Data.SqlServerCe;
using System.IO;

namespace FromSoftwareStorage
{
    public sealed class DatabaseEngineProvider
    {
        public static readonly string AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FromSoftwareGameSaves", "Storage");
        private const string DatabaseName = "Data.sdf";
        private const string RsaPrivateKeyFile = "DataPrivateKey.xml";

        public DatabaseEngineProvider()
        {
            DatabaseFileName = Path.Combine(AppDataPath, DatabaseName);
            RsaPrivateKeyFileName = Path.Combine(AppDataPath, RsaPrivateKeyFile);
        }

        public string DatabaseFileName { get; }
        public string RsaPrivateKeyFileName { get; }

        public void CreateDatabase(string password)
        {
            string connectionString = $"Data Source='{DatabaseFileName}'; LCID=1033; Password={password}; Case Sensitive = TRUE;";
            DoCreation(connectionString);

            EncryptPassword(password);
        }

        private void EncryptPassword(string password)
        {
            RsaCryptoService.GeneratePrivateKey(RsaPrivateKeyFileName);


        }

        public void CreateDatabase()
        {
            string connectionString = $"Data Source='{DatabaseFileName}'; LCID=1033; Case Sensitive = TRUE;";
            DoCreation(connectionString);
        }

        private static void DoCreation(string connectionString)
        {
            SqlCeEngine engine = new SqlCeEngine(connectionString);
            engine.CreateDatabase();
        }
    }
}

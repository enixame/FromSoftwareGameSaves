using System;
using System.Data.SqlServerCe;
using System.IO;

namespace FromSoftwareStorage
{
    public sealed class DatabaseEngineProvider
    {
        public static readonly string AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FromSoftwareGameSaves", "Storage");
        private const string DatabaseName = "Data.sdf";
        private const string BackupDatabaseName = "Data_backup.sdf";
        private const string RsaPrivateKeyFile = "DataPrivateKey.key";
        private const string RsaPublicKeyFile = "DataPublicKey.key";

        public DatabaseEngineProvider()
        {
            DatabaseFileName = Path.Combine(AppDataPath, DatabaseName);
            BackupDatabaseFileName = Path.Combine(AppDataPath, BackupDatabaseName);
            RsaPrivateKeyFileName = Path.Combine(AppDataPath, RsaPrivateKeyFile);
            RsaPublicKeyFileName = Path.Combine(AppDataPath, RsaPublicKeyFile);
        }

        private string BackupDatabaseFileName { get; }
        public string DatabaseFileName { get; }
        public string RsaPrivateKeyFileName { get; }
        public string RsaPublicKeyFileName { get; }

        public bool DataBaseExists => File.Exists(DatabaseFileName);

        public bool HasPrivateKey => File.Exists(RsaPrivateKeyFileName);

        public bool HasPublicKey => File.Exists(RsaPrivateKeyFileName);

        public void CreateDatabase(string password)
        {
            GenerateNewKeys(password);
            CreateDatabaseWithName(DatabaseFileName, password);
        }

        public void CreateDatabase()
        {
            CreateDatabaseWithName(DatabaseFileName);
        }

        public void GenerateNewKeys(string password)
        {
            if(HasPrivateKey)
                File.Delete(RsaPrivateKeyFileName);

            if(HasPublicKey)
                File.Delete(RsaPublicKeyFileName);

            EncryptPassword(password);
        }

        private void EncryptPassword(string password)
        { 
            RsaCryptoService.GeneratePrivateKey(RsaPrivateKeyFileName);

            using (RsaCryptoService cryptoService = new RsaCryptoService())
            {
                string encryptData = cryptoService.EncryptData(RsaPrivateKeyFileName, password);
                File.WriteAllText(RsaPublicKeyFileName, encryptData);
            }
        }

        private static void DoCreation(string connectionString)
        {
            SqlCeEngine engine = new SqlCeEngine(connectionString);
            engine.CreateDatabase();
        }

        private static string BuildConnectionString(string databaseName, string password = null)
        {
            SqlCeConnectionStringBuilder sqlCeConnectionStringBuilder =
                new SqlCeConnectionStringBuilder {DataSource = databaseName, InitialLcid = 1033, CaseSensitive = true};

            if (!string.IsNullOrEmpty(password) && !string.IsNullOrWhiteSpace(password))
                sqlCeConnectionStringBuilder.Password = password;

            return sqlCeConnectionStringBuilder.ConnectionString;
        }

        private static void CreateDatabaseWithName(string databaseName, string password = null)
        {
            DoCreation(BuildConnectionString(databaseName, password));
        }

    }
}

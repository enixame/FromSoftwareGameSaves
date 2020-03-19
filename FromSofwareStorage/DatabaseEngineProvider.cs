using System;
using System.Data.SqlServerCe;
using System.IO;
using System.Threading.Tasks;
using FromSoftwareStorage.Images;

namespace FromSoftwareStorage
{
    public sealed class DatabaseEngineProvider
    {
        public static readonly string AppDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "FromSoftwareGameSaves", "Storage");
        private const string DatabaseName = "Data.sdf";
        private const string BackupDatabaseName = "Data_backup.sdf";
        private const string RsaPrivateKeyFile = "DataPrivateKey.key";
        private const string RsaPublicKeyFile = "DataPublicKey.key";
        private const string DarkSouls3GameName = "DarkSouls 3";
        private const string SekiroGameName = "Sekiro";

        private readonly string[] _sqlSeparator = { "\r\nGO\r\n", "\r\nGO", "\r\nGo\r\n", "\r\nGo", "\r\ngo\r\n", "\r\ngo", "\r\ngO\r\n", "\r\ngO" };

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

        public void CreateDatabase(string password = null)
        {
            GenerateNewKeys(password);
            CreateDatabaseWithName(DatabaseFileName, password);
        }

        public void GenerateNewKeys(string password)
        {
            if (string.IsNullOrEmpty(password))
                return;

            if (string.IsNullOrWhiteSpace(password))
                return;

            if(HasPrivateKey)
                File.Delete(RsaPrivateKeyFileName);

            if(HasPublicKey)
                File.Delete(RsaPublicKeyFileName);

            EncryptPassword(password);
        }

        public async Task<bool> InstallDatabaseAsync(string password = null)
        {
            string connectionString = BuildConnectionString(DatabaseFileName, password);
            if (!DataBaseExists)
            {
                GenerateNewKeys(password);
                DoCreation(connectionString);
            }

            if (await ExecuteSql(connectionString))
            {
                return await InsertImagesAsync(connectionString);
            }

            return false;
        }

        private async Task<bool> ExecuteSql(string connectionString)
        {
            using (SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString))
            {
                sqlCeConnection.Open();

                using (SqlCeTransaction sqlCeTransaction = sqlCeConnection.BeginTransaction())
                {
                    string sqlData = await SqlEngineProvider.GetSqlDataAsync();
                    string[] sqlStatements = sqlData.Split(_sqlSeparator, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var sqlStatement in sqlStatements)
                    {
                        using (SqlCeCommand sqlCeCommand = sqlCeConnection.CreateCommand())
                        {
                            sqlCeCommand.Transaction = sqlCeTransaction;
                            sqlCeCommand.CommandText = sqlStatement;
                            sqlCeCommand.ExecuteNonQuery();
                        }
                    }

                    sqlCeTransaction.Commit();
                }

                return true;
            }
        }

        private static async Task<bool> InsertImagesAsync(string connectionString)
        {
            string sqlCommand = await SqlEngineProvider.GetImageInsertCommandAsync();

            using (SqlCeConnection sqlCeConnection = new SqlCeConnection(connectionString))
            {
                sqlCeConnection.Open();

                using (SqlCeTransaction sqlCeTransaction = sqlCeConnection.BeginTransaction())
                {
                    // dark souls3
                    byte[] darkSouls3Resource = await ImageResource.GetDarkSouls3ImageResourceAsync();
                    ExecuteSqlCommandWithParameters(DarkSouls3GameName, darkSouls3Resource, sqlCeConnection, sqlCommand, sqlCeTransaction);

                    // Sekiro
                    byte[] sekiroResource = await ImageResource.GetSekiroImageResourceAsync();
                    ExecuteSqlCommandWithParameters(SekiroGameName, sekiroResource, sqlCeConnection, sqlCommand, sqlCeTransaction);
                    
                    sqlCeTransaction.Commit();

                    return true;
                }
            }
        }

        private static void ExecuteSqlCommandWithParameters(string gameName,
            byte[] imageFile, 
            SqlCeConnection sqlCeConnection, 
            string sqlCommand,
            SqlCeTransaction sqlCeTransaction)
        {
            using (SqlCeCommand sqlCeCommand = sqlCeConnection.CreateCommand())
            {
                sqlCeCommand.CommandText = sqlCommand;
                sqlCeCommand.Parameters.AddWithValue("@GameName", gameName);
                sqlCeCommand.Parameters.AddWithValue("@ImageFile", imageFile);

                sqlCeCommand.Transaction = sqlCeTransaction;
                sqlCeCommand.ExecuteNonQuery();
            }
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

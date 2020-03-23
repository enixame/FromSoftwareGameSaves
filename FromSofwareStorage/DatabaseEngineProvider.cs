using System;
using System.Collections.Concurrent;
using System.Configuration;
using System.Data.SqlServerCe;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FromSoftwareStorage.EntityModel;

namespace FromSoftwareStorage
{
    internal sealed class DatabaseEngineProvider : IDatabaseProvider
    {
        private readonly RsaCryptoService _rsaCryptoService;
        private readonly DatabaseEngineInstaller _installer;

        private static readonly ConcurrentDictionary<string, Lazy<string>> ConnectionStringDictionary = new ConcurrentDictionary<string, Lazy<string>>();

        public DatabaseEngineProvider()
        {
            _rsaCryptoService = new RsaCryptoService();
            _installer = new DatabaseEngineInstaller();
        }

        internal static string BuildConnectionString(string databaseName, string password = null)
        {
            SqlCeConnectionStringBuilder sqlCeConnectionStringBuilder =
                new SqlCeConnectionStringBuilder {DataSource = databaseName, InitialLcid = 1033, CaseSensitive = true};

            if (!string.IsNullOrEmpty(password) && !string.IsNullOrWhiteSpace(password))
                sqlCeConnectionStringBuilder.Password = password;

            return sqlCeConnectionStringBuilder.ConnectionString;
        }

        public string DataBaseFileName => _installer.DatabaseFileName;
        public bool IsDatabaseInstalled => _installer.DataBaseExists;
        public bool HasDatabasePassword => _installer.HasPrivateKey && _installer.HasPublicKey;

        public async Task<bool> InstallAsync(string password = null)
        {
            return await _installer.InstallDatabaseAsync(password);
        }

        public DataEntities GetEntities(string connectionStringName)
        {
            var providerConnectionString = GetOrBuildProviderConnectionString(connectionStringName);
            return new DataEntities(providerConnectionString);
        }

        public string GetOrBuildProviderConnectionString(string connectionStringName)
        {
            Lazy<string> connectionStringValue = ConnectionStringDictionary.GetOrAdd(connectionStringName,
                key => new Lazy<string>(() => BuildProviderConnectionString(key), LazyThreadSafetyMode.ExecutionAndPublication));

            return connectionStringValue.Value;
        }

        private string BuildProviderConnectionString(string connectionStringName)
        {
            ConnectionStringSettings connectionStringSettings = ConfigurationManager.ConnectionStrings[connectionStringName];
            if (connectionStringSettings == null)
                throw new InvalidOperationException($"Missing connectionString name: {connectionStringName}");

            if (!_installer.DataBaseExists)
                throw new InvalidOperationException($"Database does not exist");

            string password = null;
            if (_installer.HasPrivateKey && _installer.HasPublicKey)
            {
                password = _rsaCryptoService.DecryptData(_installer.RsaPrivateKeyFileName, File.ReadAllText(_installer.RsaPublicKeyFileName));
            }

            string databaseConnectionString = BuildConnectionString(_installer.DatabaseFileName, password);
            string providerConnectionString =
                string.Format(connectionStringSettings.ConnectionString, databaseConnectionString);

            return providerConnectionString;
        }
    }
}

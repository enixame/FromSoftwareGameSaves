using System.Threading.Tasks;
using FromSoftwareStorage.EntityModel;

namespace FromSoftwareStorage
{
    public interface IDatabaseProvider
    {
        string DataBaseFileName { get; }

        bool IsDatabaseInstalled { get; }

        bool HasDatabasePassword { get; }

        Task<bool> InstallAsync(string password = null);

        DataEntities GetEntities(string connectionStringName);
    }
}

using System.Threading.Tasks;
using FromSoftwareStorage.EntityModel;

namespace FromSoftwareStorage
{
    public interface IDatabaseProvider
    {
        bool IsDatabaseInstalled { get; }

        bool HasDatabasePassword { get; }

        Task<bool> InstallAsync(string password = null);

        DataEntities GetEntities(string connectionStringName);
    }
}

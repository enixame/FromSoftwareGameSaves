using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FromSoftwareStorage
{
    internal sealed class SqlEngineProvider
    {
        private const string SqlResourceName = "FromSoftwareStorage.Sql.";
        private const string SqlInsertImageResourceName = "FromSoftwareStorage.SqlCommands.InsertImage.sql";

        public static async Task<string> GetSqlDataAsync()
        {
            ASCIIEncoding bytesConverter = new ASCIIEncoding();
            Assembly assembly = typeof(SqlEngineProvider).Assembly;

            using (MemoryStream memoryStream = new MemoryStream() )
            {
                foreach (string manifestResourceName in assembly.GetManifestResourceNames()
                    .Where(resource => resource.StartsWith(SqlResourceName, StringComparison.Ordinal))
                    .OrderBy(resource => resource))
                {
                    byte[] bytes = await ResourceStream.GetBinaryResourceAsync(manifestResourceName);
                    await memoryStream.WriteAsync(bytes, 0, bytes.Length);
                    await memoryStream.FlushAsync();
                }

                return bytesConverter.GetString(memoryStream.ToArray());
            }
        }

        public static async Task<string> GetImageInsertCommandAsync()
        {
            return await ResourceStream.GetResourceStringAsync(SqlInsertImageResourceName);
        }
    }
}

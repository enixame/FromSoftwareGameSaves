using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace FromSoftwareStorage
{
    public sealed class SqlEngineProvider
    {
        private static readonly byte[] EmptyBytes = new byte[0]; 

        private const string SqlTablesCreation = "FromSoftwareStorage.Sql.Tables.sql";
        private const string SqlInsertData = "FromSoftwareStorage.Sql.InsertData.sql";

        public static async Task<byte[]> GetSqlDataAsync()
        {
            Assembly assembly = typeof(SqlEngineProvider).Assembly;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (StreamWriter streamWriter = new StreamWriter(memoryStream))
                {
                    if (!await WriteSqlDataAsync(assembly, streamWriter, SqlTablesCreation)) return EmptyBytes;
                    if (!await WriteSqlDataAsync(assembly, streamWriter, SqlInsertData)) return EmptyBytes;

                    return memoryStream.ToArray();
                }
            }
        }

        private static async Task<bool> WriteSqlDataAsync(Assembly assembly, StreamWriter streamWriter, string sqlResource)
        {
            Stream sqlResourceStream = assembly.GetManifestResourceStream(sqlResource);
            if (sqlResourceStream == null) return false;

            using (StreamReader streamReader = new StreamReader(sqlResourceStream))
            {
                string fileContent = await streamReader.ReadToEndAsync();
                await streamWriter.WriteAsync(fileContent);
                await streamWriter.WriteLineAsync();
            }

            return true;
        }
    }
}

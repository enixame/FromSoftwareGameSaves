using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace FromSoftwareStorage
{
    public sealed class SqlEngineProvider
    {
        private static readonly byte[] EmptyBytes = new byte[0];

        private const string SqlResourceName = "FromSoftwareStorage.Sql.";
        private const string SqlInsertImageResourceName = "FromSoftwareStorage.SqlCommands.InsertImage.sql";

        public static async Task<byte[]> GetSqlDataBytesAsync()
        {
            Assembly assembly = typeof(SqlEngineProvider).Assembly;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (StreamWriter streamWriter = new StreamWriter(memoryStream))
                {
                    foreach (string manifestResourceName in assembly.GetManifestResourceNames()
                        .Where(resource => resource.StartsWith(SqlResourceName, StringComparison.Ordinal))
                        .OrderBy(resource => resource))
                    {
                        if (!await WriteSqlDataAsync(assembly, streamWriter, manifestResourceName)) return EmptyBytes;
                        await streamWriter.WriteLineAsync();
                        await streamWriter.FlushAsync();
                    }

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
                await streamWriter.FlushAsync();
            }

            return true;
        }

        public static async Task<string> GetImageInsertCommandAsync()
        {
            return await ResourceStream.GetResourceStringAsync(SqlInsertImageResourceName);
        }
    }
}

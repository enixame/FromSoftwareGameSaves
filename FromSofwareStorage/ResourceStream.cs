using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace FromSoftwareStorage
{
    public static class ResourceStream
    {
        public static async Task<string> GetResourceStringAsync(string resourceName)
        {
            Assembly assembly = typeof(ResourceStream).Assembly;
            using (Stream sqlResourceStream = assembly.GetManifestResourceStream(resourceName))
            {
                if (sqlResourceStream == null) return string.Empty;
 
                using (StreamReader streamReader = new StreamReader(sqlResourceStream))
                {
                    string fileContent = await streamReader.ReadToEndAsync();
                    return fileContent;
                }
            }
        }

        public static async Task<byte[]> GetBinaryResourceAsync(string resourceName)
        {
            Assembly assembly = typeof(ResourceStream).Assembly;
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

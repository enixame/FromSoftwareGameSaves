using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace FromSoftwareStorage
{
    public static class ResourceStream
    {
        public static async Task<string> GetResourceStringAsync(string resourceName)
        {
            Assembly assembly = typeof(ResourceStream).Assembly;
            Stream sqlResourceStream = assembly.GetManifestResourceStream(resourceName);
            if (sqlResourceStream == null) return string.Empty;

            using (StreamReader streamReader = new StreamReader(sqlResourceStream))
            {
                string fileContent = await streamReader.ReadToEndAsync();
                return fileContent;
            }
        }

        public static async Task<byte[]> GetResourceBytesAsync(string resourceName)
        {
            Assembly assembly = typeof(ResourceStream).Assembly;
            Stream sqlResourceStream = assembly.GetManifestResourceStream(resourceName);
            if (sqlResourceStream == null) return new byte[0];

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (StreamWriter streamWriter = new StreamWriter(memoryStream))
                {
                    using (StreamReader streamReader = new StreamReader(sqlResourceStream))
                    {
                        string fileContent = await streamReader.ReadToEndAsync();

                        await streamWriter.WriteAsync(fileContent);
                        await streamWriter.FlushAsync();

                        return memoryStream.ToArray();
                    }
                }
            }
            
        }
    }
}

using System.Threading.Tasks;

namespace FromSoftwareStorage.Images
{
    public static class ImageResource
    {
        private const string DarkSouls3ResourceName = "FromSoftwareStorage.Images.DarkSoulsIII.jpg";
        private const string SekiroResourceName = "FromSoftwareStorage.Images.Sekiro.jpg";

        public static async Task<byte[]> GetDarkSouls3ImageResourceAsync()
        {
            return await ResourceStream.GetResourceBytesAsync(DarkSouls3ResourceName);
        }

        public static async Task<byte[]> GetSekiroImageResourceAsync()
        {
            return await ResourceStream.GetResourceBytesAsync(SekiroResourceName);
        }
    }
}

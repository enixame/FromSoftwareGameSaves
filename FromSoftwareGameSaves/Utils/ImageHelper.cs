using System;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using FromSoftwareStorage;
using FromSoftwareStorage.EntityModel;

namespace FromSoftwareGameSaves.Utils
{
    public static class ImageHelper
    {
        private const string DefaultResourceImage = "FromSoftwareGameSaves.Images.default.png";
        private static readonly ImageSource DefaultImage;

        static ImageHelper()
        {
            Assembly assembly = typeof(ImageHelper).Assembly;
            using (Stream resourceStream = assembly.GetManifestResourceStream(DefaultResourceImage))
            {
                DefaultImage = CreateBitmapImageSourceFromStream(resourceStream);
            }
        }

        public static ImageSource BuildImageSourceFromDatabase(string gameName)
        {
            using (DataEntities dataEntities = Database.DatabaseProvider.GetEntities(ConnectionStrings.DataEntities))
            {
                var gameImage = dataEntities.Images
                    .FirstOrDefault(image => image.GameName.Equals(gameName));

                try
                {
                    return gameImage == null
                        ? DefaultImage
                        : CreateBitmapImageSourceFromBytes(gameImage.ImageFile);
                }
                catch (Exception)
                {
                    return DefaultImage;
                }
            }
        }

        public static ImageSource CreateBitmapImageSourceFromBytes(byte[] imageBytes)
        {
            using (Stream imageStream = new MemoryStream())
            {
                imageStream.Write(imageBytes, 0, imageBytes.Length);
                return CreateBitmapImageSourceFromStream(imageStream);
            }
        }

        public static ImageSource CreateBitmapImageSourceFromStream(Stream imageStream)
        {
            using (System.Drawing.Image image = System.Drawing.Image.FromStream(imageStream))
            {
                using (Stream bitmapImageStreamSource = new MemoryStream())
                {
                    image.Save(bitmapImageStreamSource, ImageFormat.Bmp);

                    bitmapImageStreamSource.Position = 0;
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.StreamSource = bitmapImageStreamSource;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                    return bitmapImage;
                }
            }
        }
    }
}

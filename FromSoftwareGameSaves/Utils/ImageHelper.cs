using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FromSoftwareGameSaves.Utils
{
    public static class ImageHelper
    {
        public const string ImageExtensionJpg = ".jpg";

        private const string ImagesPath = "../Images/";

        public static ImageSource BuildImageSource(string imageName, string imageExtension)
        {
            return new BitmapImage(new Uri(ImagesPath + imageName + imageExtension, UriKind.Relative));
        }
    }
}

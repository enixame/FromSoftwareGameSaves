using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FromSoftwareStorage.Images;
using NUnit.Framework;

namespace FromSoftwareGameSaves.Test
{
    [TestFixture]
    public class ImageResourceTests
    {
        private const string TestDirectory = "Image";
        private string _rootDirectory;

        [SetUp]
        public void Setup()
        {
            string directoryName = Path.GetDirectoryName(new Uri(typeof(FromSoftwareGamePathTests).Assembly.CodeBase).LocalPath);
            _rootDirectory = Path.Combine(directoryName ?? throw new InvalidOperationException(), TestDirectory);

            if (!Directory.Exists(_rootDirectory))
                Directory.CreateDirectory(_rootDirectory);
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(_rootDirectory))
                Directory.Delete(_rootDirectory);
        }

        [Test]
        public async Task DarkSouls3ImageResourceTests()
        {
            byte[] bytes  = await ImageResource.GetDarkSouls3ImageResourceAsync();
            string imageName = Path.Combine(_rootDirectory, "DS3.jpg");
          
            File.WriteAllBytes(imageName, bytes);
            Assert.That(File.Exists(imageName), Is.True);
            Assert.That(GetMimeTypeFromImageByteArray(bytes), Is.EqualTo("image/jpeg"));

            File.Delete(imageName);
        }

        [Test]
        public async Task SekiroImageResourceTests()
        {
            byte[] bytes = await ImageResource.GetSekiroImageResourceAsync();
            string imageName = Path.Combine(_rootDirectory, "Sekiro.jpg");

            File.WriteAllBytes(imageName, bytes);

            Assert.That(File.Exists(imageName), Is.True);
            Assert.That(GetMimeTypeFromImageByteArray(bytes), Is.EqualTo("image/jpeg"));

            File.Delete(imageName);
        }

        public static string GetMimeTypeFromImageByteArray(byte[] byteArray)
        {
            using (MemoryStream memoryStream = new MemoryStream(byteArray))
            using (Image image = Image.FromStream(memoryStream))
            {
                return ImageCodecInfo.GetImageEncoders().First(codec => codec.FormatID == image.RawFormat.Guid).MimeType;
            }
        }
    }
}

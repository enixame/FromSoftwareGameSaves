using System;
using System.Security.Cryptography;
using System.Text;

namespace FromSoftwareStorage
{
    internal sealed class RsaCryptoService : IDisposable
    {
        private const int DwKeySize = 2048;

        private readonly RSACryptoServiceProvider _rsaCryptoServiceProvider;
        private readonly UnicodeEncoding _byteConverter = new UnicodeEncoding();

        internal static void GeneratePrivateKey(string path)
        {
            using (RSACryptoServiceProvider rsaCryptoServiceProvider = new RSACryptoServiceProvider(DwKeySize))
                System.IO.File.WriteAllText(path, rsaCryptoServiceProvider.ToXmlString(true));
        }

        public RsaCryptoService()
        {
            _rsaCryptoServiceProvider = new RSACryptoServiceProvider(DwKeySize);
        }

        public string EncryptData(string path, string data)
        {
            string base64String = null;
            try
            {
                byte[] dataToEncrypt = _byteConverter.GetBytes(data);

                _rsaCryptoServiceProvider.FromXmlString(System.IO.File.ReadAllText(path));
                byte[] encryptedData = _rsaCryptoServiceProvider.Encrypt(dataToEncrypt, true);
                base64String = Convert.ToBase64String(encryptedData);
            }
            finally
            {
                _rsaCryptoServiceProvider.PersistKeyInCsp = false;
            }

            return base64String;
        }

        public string DecryptData(string path, string data)
        {
            string decryptedData = null;
            try
            {
                _rsaCryptoServiceProvider.FromXmlString(System.IO.File.ReadAllText(path));
                byte[] base64String = Convert.FromBase64String(data);
                byte[] bytes = _rsaCryptoServiceProvider.Decrypt(base64String, true);

                decryptedData = _byteConverter.GetString(bytes);
            }
            finally
            {
                _rsaCryptoServiceProvider.PersistKeyInCsp = false;
            }

            return decryptedData;
        }

        public void Dispose()
        {
            _rsaCryptoServiceProvider?.Dispose();
        }
    }
}

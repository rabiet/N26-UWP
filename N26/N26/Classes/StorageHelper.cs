using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Security.Credentials;
using Windows.Security.Cryptography;
using Windows.Security.Cryptography.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace N26.Classes
{
    class StorageHelper
    {
        StorageFolder cache = ApplicationData.Current.LocalFolder;
        ApplicationDataContainer localSettings = ApplicationData.Current.LocalSettings;
        StorageFolder localFolder = ApplicationData.Current.LocalFolder;
        private readonly SymmetricKeyAlgorithmProvider cryptingProvider;


        public StorageHelper()
        {
            cryptingProvider = SymmetricKeyAlgorithmProvider.OpenAlgorithm(SymmetricAlgorithmNames.AesEcbPkcs7);
        }

        private static IBuffer GetHash(string key)
        {
            IBuffer buffUtf8Msg = CryptographicBuffer.ConvertStringToBinary(key, BinaryStringEncoding.Utf8);
            HashAlgorithmProvider objAlgProv = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha512);
            IBuffer buffHash = objAlgProv.HashData(buffUtf8Msg);
            if (buffHash.Length != objAlgProv.HashLength)
            {
                throw new Exception("There was an error creating the hash");
            }
            return buffHash;
        }

        private string getUserPassword()
        {
            var loginCredential = GetCredentialFromLocker();

            if (loginCredential != null)
                loginCredential.RetrievePassword();
            else
                return null;

            return loginCredential.Password;
        }

        private PasswordCredential GetCredentialFromLocker()
        {
            try
            {
                var vault = new PasswordVault();
                var credentialList = vault.FindAllByResource("N26");
                if (credentialList.Count > 0)
                {
                    PasswordCredential credential = credentialList[0];
                    return credential;
                }
            }
            catch (Exception) { }
            return null;
        }

        public string Encrypt(string data)
        {
            IBuffer binaryData = CryptographicBuffer.ConvertStringToBinary(data, BinaryStringEncoding.Utf8);// Encoding.Unicode.GetBytes(data).AsBuffer();
            CryptographicKey key = cryptingProvider.CreateSymmetricKey(GetHash(getUserPassword()));
            IBuffer encryptedBinaryData = CryptographicEngine.Encrypt(key, binaryData, null);

            return CryptographicBuffer.EncodeToBase64String(encryptedBinaryData);
        }

        public string Decrypt(string encryptedData)
        {
            IBuffer binaryData = CryptographicBuffer.DecodeFromBase64String(encryptedData);
            CryptographicKey key = cryptingProvider.CreateSymmetricKey(GetHash(getUserPassword()));
            IBuffer decryptedData = CryptographicEngine.Decrypt(key, binaryData, null);

            return CryptographicBuffer.ConvertBinaryToString(BinaryStringEncoding.Utf8, decryptedData);
        }

        public async void WriteValue(string fileName, string value)
        {
            IBuffer iv = CryptographicBuffer.GenerateRandom(cryptingProvider.BlockLength);

            localSettings.Values[fileName] = Encoding.Unicode.GetString(iv.ToArray());

            StorageFile toWrite = await cache.CreateFileAsync(string.Format("{0}.txt", fileName), CreationCollisionOption.ReplaceExisting);
            await FileIO.WriteTextAsync(toWrite, Encrypt(value));
        }

        public async Task<string> ReadValue(string fileName)
        {
            IBuffer iv = CryptographicBuffer.ConvertStringToBinary((string) localSettings.Values[fileName], BinaryStringEncoding.Utf16BE);
            StorageFile toLoad = await cache.GetFileAsync(string.Format("{0}.txt", fileName));
            return Decrypt(await FileIO.ReadTextAsync(toLoad));
        }
    }
}

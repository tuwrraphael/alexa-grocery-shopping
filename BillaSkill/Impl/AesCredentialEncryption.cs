using BillaSkill.Models;
using BillaSkill.Services;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace BillaSkill.Impl
{
    public class AesCredentialEncryption : ICredentialEncryption
    {
        private readonly IAESKeyProvider keyProvider;

        private async Task<byte[]> Encrypt(ICryptoTransform encryptor, string plain)
        {
            using (var resultStream = new MemoryStream())
            {
                using (var aesStream = new CryptoStream(resultStream, encryptor, CryptoStreamMode.Write))
                using (var wr = new StreamWriter(aesStream))
                {
                    await wr.WriteAsync(plain);
                    await wr.FlushAsync();
                }
                return resultStream.ToArray();
            }
        }

        private async Task<string> Decrypt(ICryptoTransform decryptor, byte[] encrypted)
        {
            using (var input = new MemoryStream(encrypted))
            {
                using (var aesStream = new CryptoStream(input, decryptor, CryptoStreamMode.Read))
                using (var reader = new StreamReader(aesStream))
                {
                    return await reader.ReadToEndAsync();
                }
            }
        }

        public AesCredentialEncryption(IAESKeyProvider keyProvider)
        {
            this.keyProvider = keyProvider;
        }

        public async Task<LieferantCredentials> Decrypt(EncryptedCredentials credentials)
        {
            var key = await keyProvider.GetKey();
            var mailbytes = Convert.FromBase64String(credentials.L_MAIL);

            using (var aes = Aes.Create())
            {
                var iv = new byte[aes.IV.Length];
                Buffer.BlockCopy(mailbytes, 0, iv, 0, iv.Length);
                var maildec = new byte[mailbytes.Length - iv.Length];
                Buffer.BlockCopy(mailbytes, iv.Length, maildec, 0, maildec.Length);
                using (var decryptor = aes.CreateDecryptor(key, iv))
                {
                    return new LieferantCredentials()
                    {
                        L_MAIL = await Decrypt(decryptor, maildec),
                        L_PASSWORD = await Decrypt(decryptor, Convert.FromBase64String(credentials.L_PASSWORD))
                    };
                }
            }
        }

        public async Task<EncryptedCredentials> Encrypt(LieferantCredentials credentials)
        {
            var key = await keyProvider.GetKey();
            using (var aes = Aes.Create())
            using (var encryptor = aes.CreateEncryptor(key, aes.IV))
            {
                var mailenc = await Encrypt(encryptor, credentials.L_MAIL);
                byte[] res = new byte[mailenc.Length + aes.IV.Length];
                Buffer.BlockCopy(aes.IV, 0, res, 0, aes.IV.Length);
                Buffer.BlockCopy(mailenc, 0, res, aes.IV.Length, mailenc.Length);
                return new EncryptedCredentials()
                {
                    L_MAIL = Convert.ToBase64String(res),
                    L_PASSWORD = Convert.ToBase64String(await Encrypt(encryptor, credentials.L_PASSWORD))
                };
            }
        }
    }
}

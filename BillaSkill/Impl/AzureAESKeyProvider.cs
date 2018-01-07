using BillaSkill.Models;
using BillaSkill.Services;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Extensions.Options;
using System;
using System.Threading.Tasks;

namespace BillaSkill.Impl
{
    public class AzureAESKeyProvider : IAESKeyProvider
    {
        private readonly AzureAesKeyOptions options;

        public AzureAESKeyProvider(IOptions<AzureAesKeyOptions> optionsAccessor)
        {
            this.options = optionsAccessor.Value;
        }
        public async Task<byte[]> GetKey()
        {
            AzureServiceTokenProvider azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));
            var secret = await keyVaultClient.GetSecretAsync(options.VAULT_URL);
            return Convert.FromBase64String(secret.Value);
        }
    }
}

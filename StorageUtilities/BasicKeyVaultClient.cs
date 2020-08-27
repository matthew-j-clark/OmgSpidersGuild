using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StorageUtilities
{
    public class BasicKeyVaultClient
    {
        public BasicKeyVaultClient()
        {
            SecretClientOptions options = new SecretClientOptions()
            {
                Retry =
                {
                    Delay= TimeSpan.FromSeconds(2),
                    MaxDelay = TimeSpan.FromSeconds(16),
                    MaxRetries = 5,
                    Mode = RetryMode.Exponential
                }
            };
            this.Client = new SecretClient(new Uri("https://omgspiderskv.vault.azure.net/"), new DefaultAzureCredential(), options);

        }

        public async Task<string> GetSecret(string secretName)
        {
            var result =  await Client.GetSecretAsync(secretName);
            return result.Value.Value;
        }

        private SecretClient Client { get; }
    }
}

using BillaSkill.Models;
using System.Threading.Tasks;

namespace BillaSkill.Services
{
    public interface ICredentialEncryption
    {
        Task<EncryptedCredentials> Encrypt(LieferantCredentials credentials);
        Task<LieferantCredentials> Decrypt(EncryptedCredentials credentials);
    }
}

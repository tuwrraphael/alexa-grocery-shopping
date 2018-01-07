using System;
using System.Threading.Tasks;

namespace BillaSkill.Services
{
    public interface IAESKeyProvider
    {
        Task<byte[]> GetKey();
    }
}

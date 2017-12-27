using BillaSkill.Models;
using System.Threading.Tasks;

namespace BillaSkill.Services
{
    public interface ISucheRepository
    {
        Task<Suche> SucheZwischenSpeichernAsync(Ware[] waren, string sessionId);
        Task<Ware> NaechsteWare(string sessionId);
        Task<Suche> GetSucheAsync(string sessionId);
        Task DeleteSucheAsync(string sessionId);
    }
}

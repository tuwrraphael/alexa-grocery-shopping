using BillaSkill.Models;
using System.Threading.Tasks;

namespace BillaSkill.Services
{
    public interface IWarenkorbRepository
    {
        Task<Warenkorb> WareHinzufuegenAsync(Ware ware, string userId);
        Task<Warenkorb> GetForUserAsync(string userId);
        Task ClearAsync(string userId);
    }
}

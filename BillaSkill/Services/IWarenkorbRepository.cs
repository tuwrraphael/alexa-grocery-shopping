using BillaSkill.Models;
using System.Threading.Tasks;

namespace BillaSkill.Services
{
    public interface IWarenkorbRepository
    {
        Task<Warenkorb> WareHinzufuegen(Ware ware, string userId);
    }
}

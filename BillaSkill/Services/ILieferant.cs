using BillaSkill.Models;
using System.Threading.Tasks;

namespace BillaSkill.Services
{
    public interface ILieferant
    {
        Task<Ware[]> SearchAsync(string term);
    }
}

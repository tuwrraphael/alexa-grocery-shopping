using BillaSkill.Models;
using System.Threading.Tasks;

namespace BillaSkill.Services
{
    public interface IUserRepository
    {
        Task CreateAsync(User user);
        Task<User> GetAsync(string key);
    }
}

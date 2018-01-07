using BillaSkill.Models;
using BillaSkill.Services;
using System.Threading.Tasks;

namespace BillaSkill.Impl.FileStore
{
    public class FileStoreUserRepository : IUserRepository
    {
        private readonly IFileAccess access;
        private const string Collection = "User";

        public FileStoreUserRepository(IFileAccess access)
        {
            this.access = access;
        }

        public async Task CreateAsync(User user)
        {
            await access.SaveAsync(user.Key, Collection, user);
        }

        public async Task<User> GetAsync(string key)
        {
            return await access.GetAsync<User>(key, Collection);
        }
    }
}

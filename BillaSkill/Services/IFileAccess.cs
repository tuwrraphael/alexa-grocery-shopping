using System.Threading.Tasks;

namespace BillaSkill.Services
{
    public interface IFileAccess
    {
        Task InitializeAsync();
        Task DeleteAsync(string id, string collection);
        Task<T> GetAsync<T>(string id, string collection) where T:class;
        Task<string> SaveAsync<T>(string id, string collection, T t);
    }
}

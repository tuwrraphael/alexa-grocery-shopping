using BillaSkill.Models;
using BillaSkill.Services;
using System.Threading.Tasks;

namespace BillaSkill.Impl.FileStore
{
    public class FileStoreSucheRepository : ISucheRepository
    {
        private readonly IFileAccess access;
        private const string Collection = "Suche";

        public FileStoreSucheRepository(IFileAccess access)
        {
            this.access = access;
        }

        public async Task DeleteSucheAsync(string sessionId)
        {
            await access.DeleteAsync(sessionId, Collection);
        }

        public async Task<Suche> GetSucheAsync(string sessionId)
        {
            return await access.GetAsync<Suche>(sessionId, Collection);
        }

        public async Task<Ware> NaechsteWare(string sessionId)
        {
            var suche = await access.GetAsync<Suche>(sessionId, Collection);
            if (null == suche)
            {
                return null;
            }
            if (suche.Position == (suche.Waren.Length - 1))
            {
                return null;
            }
            suche.Position++;
            await access.SaveAsync(sessionId, Collection, suche);
            return suche.Waren[suche.Position];
        }

        public async Task<Suche> SucheZwischenSpeichernAsync(Ware[] waren, string sessionId)
        {
            var suche = new Suche()
            {
                Position = 0,
                SessionId = sessionId,
                Waren = waren
            };
            var id = await access.SaveAsync(sessionId, Collection, suche);
            return suche;
        }
    }
}

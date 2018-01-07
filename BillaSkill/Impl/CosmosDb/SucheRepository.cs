using BillaSkill.Models;
using BillaSkill.Services;
using Microsoft.Azure.Documents.Client;
using System.Linq;
using System.Threading.Tasks;

namespace BillaSkill.Impl.CosmosDb
{
    public class SucheRepository : ISucheRepository
    {
        private readonly IDbAccess dbAccess;

        public SucheRepository(IDbAccess dbAccess)
        {
            this.dbAccess = dbAccess;
        }

        public async Task DeleteSucheAsync(string sessionId)
        {
            var client = dbAccess.GetClient();
            var suchen = client.CreateDocumentQuery<Suche>(UriFactory.CreateDocumentCollectionUri(DbAccess.DBName, DbAccess.SucheCollectionName))
                .Where(p => p.SessionId == sessionId);
            foreach (var s in suchen)
            {
                await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(DbAccess.DBName, DbAccess.SucheCollectionName, s.Id));
            }
        }

        public async Task<Suche> GetSucheAsync(string sessionId)
        {
            var client = dbAccess.GetClient();
            var suchen = client.CreateDocumentQuery<Suche>(UriFactory.CreateDocumentCollectionUri(DbAccess.DBName, DbAccess.SucheCollectionName))
                .Where(p => p.SessionId == sessionId);
            return suchen.ToArray().FirstOrDefault();
        }

        public async Task<Ware> NaechsteWare(string sessionId)
        {
            var client = dbAccess.GetClient();
            var suche = client.CreateDocumentQuery<Suche>(UriFactory.CreateDocumentCollectionUri(DbAccess.DBName, DbAccess.SucheCollectionName))
                .Where(p => p.SessionId == sessionId).ToArray().FirstOrDefault();
            if( null == suche)
            {
                return null;
            }
            if (suche.Position == (suche.Waren.Length -1))
            {
                return null;
            }
            suche.Position++;
            await client.ReplaceDocumentAsync(
                    UriFactory.CreateDocumentUri(DbAccess.DBName, DbAccess.SucheCollectionName, suche.Id), suche);
            return suche.Waren[suche.Position];
        }

        public async Task<Suche> SucheZwischenSpeichernAsync(Ware[] waren, string sessionId)
        {
            await DeleteSucheAsync(sessionId);
            var client = dbAccess.GetClient();
            var suche = new Suche()
            {
                Position = 0,
                SessionId = sessionId,
                Waren = waren
            };
            var doc = await client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(DbAccess.DBName, DbAccess.SucheCollectionName), suche);
            suche.Id = doc.Resource.Id;
            return suche;
        }
    }
}

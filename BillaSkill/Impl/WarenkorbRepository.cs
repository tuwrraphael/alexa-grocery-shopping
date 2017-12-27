using BillaSkill.Models;
using BillaSkill.Services;
using Microsoft.Azure.Documents.Client;
using System.Linq;
using System.Threading.Tasks;

namespace BillaSkill.Impl
{
    public class WarenkorbRepository : IWarenkorbRepository
    {
        private readonly IDbAccess dbAccess;

        public WarenkorbRepository(IDbAccess dbAccess)
        {
            this.dbAccess = dbAccess;
        }

        public async Task<Warenkorb> GetForUserAsync(string userId)
        {
            var client = dbAccess.GetClient();
            var warenKorb = client.CreateDocumentQuery<Warenkorb>(dbAccess.GetWarenkorbCollectionUri()).Where(p => p.UserId == userId).ToArray().FirstOrDefault();
            return warenKorb;
        }

        public async Task<Warenkorb> WareHinzufuegenAsync(Ware ware, string userId)
        {
            var client = dbAccess.GetClient();
            var warenKorb = client.CreateDocumentQuery<Warenkorb>(dbAccess.GetWarenkorbCollectionUri()).Where(p => p.UserId == userId).ToArray().FirstOrDefault();
            if (null == warenKorb)
            {
                warenKorb = new Warenkorb()
                {
                    UserId = userId,
                    Waren = new[] { new WarenkorbEintrag() {
                        Ware = ware,
                        Ammount = 1 } }
                };
                var doc = await client.CreateDocumentAsync(dbAccess.GetWarenkorbCollectionUri(), warenKorb);
                warenKorb.Id = doc.Resource.Id;
                return warenKorb;
            }
            else
            {
                var eintrag = warenKorb.Waren.Where(p => p.Ware.LieferantenID == ware.LieferantenID).FirstOrDefault();
                if (null != eintrag)
                {
                    eintrag.Ammount++;
                }
                else
                {
                    warenKorb.Waren = warenKorb.Waren.Union(new[] {new WarenkorbEintrag() {
                        Ware = ware,
                        Ammount = 1 } }).ToArray();
                }
                await client.ReplaceDocumentAsync(
                    UriFactory.CreateDocumentUri(DbAccess.DBName, DbAccess.WarenkorbCollectionName, warenKorb.Id), warenKorb);
            }
            return warenKorb;
        }
    }
}

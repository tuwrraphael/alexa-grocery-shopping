using BillaSkill.Models;
using BillaSkill.Services;
using System.Linq;
using System.Threading.Tasks;

namespace BillaSkill.Impl.FileStore
{
    public class FileStoreWarenkorbRepository : IWarenkorbRepository
    {
        private readonly IFileAccess fileAccess;
        private const string Collection = "warenkorb";

        public FileStoreWarenkorbRepository(IFileAccess fileAccess)
        {
            this.fileAccess = fileAccess;
        }

        public async Task ClearAsync(string userId)
        {
            await fileAccess.DeleteAsync(userId, Collection);
        }

        public async Task<Warenkorb> GetForUserAsync(string userId)
        {
            return await fileAccess.GetAsync<Warenkorb>(userId, Collection);
        }

        public async Task<Warenkorb> WareHinzufuegenAsync(Ware ware, string userId)
        {
            var warenKorb = await fileAccess.GetAsync<Warenkorb>(userId, Collection);
            if (null == warenKorb)
            {
                warenKorb = new Warenkorb()
                {
                    UserId = userId,
                    Waren = new[] { new WarenkorbEintrag() {
                        Ware = ware,
                        Ammount = 1 } }
                };
                var id = await fileAccess.SaveAsync(userId, Collection, warenKorb);
                warenKorb.Id = id;
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
                await fileAccess.SaveAsync(userId, Collection, warenKorb);
            }
            return warenKorb;
        }
    }
}

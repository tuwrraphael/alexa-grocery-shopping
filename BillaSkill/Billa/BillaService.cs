using BillaSkill.Models;
using BillaSkill.Services;
using Newtonsoft.Json;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BillaSkill.Billa
{
    public class BillaService : ILieferant
    {
        public async Task<Ware[]> SearchAsync(string term)
        {
            var storeId = "00-2808";
            var client = new HttpClient();
            var resText = await client.GetStringAsync($"https://shop.billa.at/api/search/full?category=&searchTerm={term}&storeId={storeId}");
            var result = JsonConvert.DeserializeObject<BillaSearchResult>(resText);
            return result.tiles.Select(BillaSearchResultExtension.ToWare).ToArray();
        }
    }
}
using BillaSkill.Models;
using BillaSkill.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BillaSkill.Controllers
{
    [Route("api/[controller]")]
    public class EinkaufController : Controller
    {
        private readonly ILieferant lieferant;
        private readonly IWarenkorbRepository repository;

        public EinkaufController(ILieferant lieferant, IWarenkorbRepository repository)
        {
            this.lieferant = lieferant;
            this.repository = repository;
        }

        [HttpGet]
        public async Task<Ware[]> Search(string term)
        {
            return await lieferant.SearchAsync(term, "00-2808");
        }

        [HttpPost]
        public async Task<Warenkorb> Example() {
            return await repository.WareHinzufuegenAsync(new Ware()
            {
                LieferantenID = "2123",
                Marke = "Coca Cola",
                Name = "Coca Cola",
                Menge = "0.5 Liter",
                Preis = 1.23f
            }, "test");
        }
    }
}

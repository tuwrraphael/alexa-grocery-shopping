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

        public EinkaufController(ILieferant lieferant)
        {
            this.lieferant = lieferant;
        }

        [HttpGet]
        public async Task<Ware[]> Search(string term)
        {
            return await lieferant.SearchAsync(term);
        }
    }
}

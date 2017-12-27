using BillaSkill.Models;
using BillaSkill.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace BillaSkill.Controllers
{
    [Route("api/[controller]")]
    public class BillaController : Controller
    {
        private readonly ILieferant lieferant;
        private readonly LieferantCredentials lieferantCredentials;

        public BillaController(ILieferant lieferant, IOptions<LieferantCredentials> credentialAccessor)
        {
            this.lieferant = lieferant;
            lieferantCredentials = credentialAccessor.Value;
        }

        [HttpGet]
        public async Task Move()
        {
            await lieferant.WarenkorbErstellenAsync(lieferantCredentials, new Warenkorb()
            {
                Waren = new[] {
                new WarenkorbEintrag() {
                    Ware = new Ware{
                    LieferantenID = "00-415299"
                    },
                    Ammount = 1
                }
            }
            });

        }
    }
}

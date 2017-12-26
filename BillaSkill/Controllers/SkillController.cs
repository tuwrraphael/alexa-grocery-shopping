
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using BillaSkill.Services;
using Microsoft.AspNetCore.Mvc;

namespace BillaSkill.Controllers
{
    [Route("api/[controller]")]
    public class SkillController : Controller
    {
        private readonly ILieferant lieferant;
        private readonly IWarenFormatter warenFormatter;

        public SkillController(ILieferant lieferant, IWarenFormatter warenFormatter)
        {
            this.lieferant = lieferant;
            this.warenFormatter = warenFormatter;
        }
        
        [HttpPost]
        public async Task<SkillResponse> Post([FromBody]SkillRequest input)
        {
            var requesttype = input.GetRequestType();
            if (requesttype == typeof(IntentRequest))
            {
                var intentRequest = input.Request as IntentRequest;
                var name = intentRequest.Intent.Name;
                switch (name) {
                    case "Warensuche":
                        var textAmmount = 3;
                        var suchbegriff = intentRequest.Intent.Slots["Suchbegriff"].Value;
                        var waren = await lieferant.SearchAsync(suchbegriff);
                        var warenText = string.Join(", ", waren.Take(textAmmount).Select(warenFormatter.Format));
                        var res = $"Beim Billa gibt es: {warenText}, und {waren.Length - textAmmount} weitere Artikel.";
                        return ResponseBuilder.Tell(res);
                    case "WarenkorbAuflisten":
                        return ResponseBuilder.Tell("Du hast nichts in deinem Warenkorb.");
                    default:
                        return ResponseBuilder.Tell("Ich weiß nicht ob es das beim Billa gibt.");
                }
            }
            //else if (requesttype == typeof(LaunchRequest))
            //{
            //    var launchRequest = input.Request as LaunchRequest;
            //    return ResponseBuilder.TellWithReprompt
            //}
            return ResponseBuilder.Tell($"Anderer Requesttyp");
        }

      
    }
}

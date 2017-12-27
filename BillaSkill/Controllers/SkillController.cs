
using System;
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
        private readonly IWarenkorbRepository warenkorbRepository;
        private readonly ISucheRepository sucheRepository;

        public SkillController(ILieferant lieferant, 
            IWarenFormatter warenFormatter,
            IWarenkorbRepository warenkorbRepository,
            ISucheRepository sucheRepository)
        {
            this.lieferant = lieferant;
            this.warenFormatter = warenFormatter;
            this.warenkorbRepository = warenkorbRepository;
            this.sucheRepository = sucheRepository;
        }
        private static Random random = new Random();
        private static string RandomInsert(string str)
        {
            return random.Next(0, 2) == 0 ? $"{str} " : string.Empty;
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
                        var suchbegriff = intentRequest.Intent.Slots["Suchbegriff"].Value;
                        var waren = await lieferant.SearchAsync(suchbegriff);
                        if (waren.Length == 0)
                        {
                            return ResponseBuilder.Tell($"Ich glaube {suchbegriff} gibt es beim Billa nicht.");
                        }
                        await sucheRepository.SucheZwischenSpeichernAsync(waren, input.Session.SessionId);
                        var ware = warenFormatter.Format(waren[0]);
                        return ResponseBuilder.Ask($"Ich habe {waren.Length} Artikel gefunden. Möchtest du {ware}?", new Reprompt()
                        {
                            OutputSpeech = new PlainTextOutputSpeech()
                            {
                                Text = $"Soll ich {waren[0].Name} hinzufügen?"
                            }
                        });
                    case "AMAZON.YesIntent":
                        var suche = await sucheRepository.GetSucheAsync(input.Session.SessionId);
                        if (null == suche)
                        {
                            return ResponseBuilder.Tell("Ich weiß nicht was hinzugefügt werden soll.");
                        }else
                        {
                            var hinzu = suche.Waren[suche.Position];
                            await warenkorbRepository.WareHinzufuegenAsync(hinzu, input.Session.User.UserId);
                            await sucheRepository.DeleteSucheAsync(input.Session.SessionId);
                            return ResponseBuilder.Tell($"{hinzu.Name}, wurde hinzugefügt.");
                        }
                    case "AMAZON.NoIntent":
                        await sucheRepository.DeleteSucheAsync(input.Session.SessionId);
                        return ResponseBuilder.Tell("Ok.");
                    case "AMAZON.NextIntent":
                        var naechsteWare = await  sucheRepository.NaechsteWare(input.Session.SessionId);
                        if (null == naechsteWare)
                        {
                            await sucheRepository.DeleteSucheAsync(input.Session.SessionId);
                            return ResponseBuilder.Tell("Es wird nichts zum Warenkorb hinzugefügt.");
                        }
                        else
                        {
                            return ResponseBuilder.Ask($"Möchtest du {RandomInsert("lieber")}{warenFormatter.Format(naechsteWare)}?", new Reprompt()
                            {
                                OutputSpeech = new PlainTextOutputSpeech()
                                {
                                    Text = $"Soll ich {naechsteWare.Name} hinzufügen?"
                                }
                            });
                        }
                    case "WarenkorbAuflisten":
                        var warenkorb = await warenkorbRepository.GetForUserAsync(input.Session.User.UserId);
                        if (null == warenkorb || 0 == warenkorb.Waren.Length)
                        {
                            return ResponseBuilder.Tell("Du hast nichts in deinem Warenkorb.");
                        } else
                        {
                            return ResponseBuilder.Tell($"In deinem Warenkorb ist {warenFormatter.Format(warenkorb)}.");
                        }
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


using System;
using System.Linq;
using System.Threading.Tasks;
using Alexa.NET;
using Alexa.NET.Request;
using Alexa.NET.Request.Type;
using Alexa.NET.Response;
using BillaSkill.Models;
using BillaSkill.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace BillaSkill.Controllers
{
    [Route("api/[controller]")]
    public class SkillController : Controller
    {
        private readonly ILieferant lieferant;
        private readonly IWarenFormatter warenFormatter;
        private readonly IWarenkorbRepository warenkorbRepository;
        private readonly ISucheRepository sucheRepository;
        private readonly LieferantCredentials credentials;

        public SkillController(ILieferant lieferant,
            IWarenFormatter warenFormatter,
            IWarenkorbRepository warenkorbRepository,
            ISucheRepository sucheRepository,
            IOptions<LieferantCredentials> credentialAccessor)
        {
            this.lieferant = lieferant;
            this.warenFormatter = warenFormatter;
            this.warenkorbRepository = warenkorbRepository;
            this.sucheRepository = sucheRepository;
            credentials = credentialAccessor.Value;
        }
        private static Random random = new Random();
        private static string RandomInsert(string str)
        {
            return random.Next(0, 2) == 0 ? $"{str} " : string.Empty;
        }

        private static float CalcPrice(Warenkorb korb)
        {
            return korb.Waren.Select(p => p.Ammount * p.Ware.Preis).Sum();
        }

        [HttpPost]
        public async Task<SkillResponse> Post([FromBody]SkillRequest input)
        {
            var requesttype = input.GetRequestType();
            if (requesttype == typeof(IntentRequest))
            {
                var intentRequest = input.Request as IntentRequest;
                var name = intentRequest.Intent.Name;
                switch (name)
                {
                    case "Warensuche":
                        var suchbegriff = intentRequest.Intent.Slots["Suchbegriff"].Value;
                        if (null == suchbegriff)
                        {
                            return ResponseBuilder.Tell($"Ich weiß nicht wie ich das suchen soll.");
                        }
                        var waren = await lieferant.SearchAsync(suchbegriff, credentials.L_STOREID);
                        if (waren.Length == 0)
                        {
                            return ResponseBuilder.Tell($"Ich glaube {suchbegriff} gibt es beim Billa nicht.");
                        }
                        await sucheRepository.SucheZwischenSpeichernAsync(waren, input.Session.SessionId);
                        var ware = warenFormatter.Format(waren[0]);
                        return ResponseBuilder.Ask($"Ich habe {(waren.Length == 1 ? "einen" : $"{waren.Length}")} Artikel gefunden. Möchtest du {ware}?", new Reprompt()
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
                        }
                        else
                        {
                            var hinzu = suche.Waren[suche.Position];
                            await warenkorbRepository.WareHinzufuegenAsync(hinzu, input.Session.User.UserId);
                            await sucheRepository.DeleteSucheAsync(input.Session.SessionId);
                            return ResponseBuilder.Ask($"{hinzu.Name} wurde hinzugefügt. Was soll ich jetzt Suchen?", new Reprompt()
                            {
                                OutputSpeech = new PlainTextOutputSpeech()
                                {
                                    Text = $"Was soll ich als nächstes Suchen?"
                                }
                            });
                        }
                    case "AMAZON.NoIntent":
                        await sucheRepository.DeleteSucheAsync(input.Session.SessionId);
                        return ResponseBuilder.Tell("Ok.");
                    case "AMAZON.NextIntent":
                        var naechsteWare = await sucheRepository.NaechsteWare(input.Session.SessionId);
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
                        }
                        else
                        {
                            return ResponseBuilder.Tell($"In deinem Warenkorb ist {warenFormatter.Format(warenkorb)}. Der Gesamtpreis ist {CalcPrice(warenkorb)}€.");
                        }
                    case "WarenkorbErstellen":
                        var korb = await warenkorbRepository.GetForUserAsync(input.Session.User.UserId);
                        if (null == korb || 0 == korb.Waren.Length)
                        {
                            return ResponseBuilder.Tell("Du hast nichts in deinem Warenkorb.");
                        }else
                        {
                            await lieferant.WarenkorbErstellenAsync(credentials, korb);
                            await warenkorbRepository.ClearAsync(input.Session.User.UserId);
                            var anzahl = korb.Waren.Length == 1 ? "wurde ein" : $"wurden {korb.Waren.Length}";
                            return ResponseBuilder.Tell($"Es {anzahl} Artikel an den Billa Onlineshop gesendet.");
                        }
                    case "AMAZON.StopIntent":
                    case "AMAZON.CancelIntent":
                        await sucheRepository.DeleteSucheAsync(input.Session.SessionId);
                        return ResponseBuilder.Empty();
                    case "AMAZON.HelpIntent":
                        return ResponseBuilder.Empty();
                    default:
                        return ResponseBuilder.Tell("Ich weiß nicht ob es das beim Billa gibt.");
                }
            }
            else if (requesttype == typeof(LaunchRequest))
            {
                return ResponseBuilder.Ask($"Billa Onlineshop. Du kannst nach Waren suchen und zu deinem Warenkorb hinzufügen.", new Reprompt()
                {
                    OutputSpeech = new PlainTextOutputSpeech()
                    {
                        Text = $"Was möchtest du einkaufen?"
                    }
                });
            }
            return ResponseBuilder.Tell($"Anderer Requesttyp");
        }


    }
}

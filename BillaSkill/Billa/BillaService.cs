using BillaSkill.Models;
using BillaSkill.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using System.Text;

namespace BillaSkill.Billa
{
    public class BillaService : ILieferant
    {
        private readonly ICredentialEncryption credentialEncryption;

        public BillaService(ICredentialEncryption credentialEncryption)
        {
            this.credentialEncryption = credentialEncryption;
        }

        public async Task<Ware[]> SearchAsync(string term, string storeId)
        {
            var client = new HttpClient();
            var resText = await client.GetStringAsync($"https://shop.billa.at/api/search/full?category=&searchTerm={term}&storeId={storeId}");
            var result = JsonConvert.DeserializeObject<BillaSearchResult>(resText);
            return result.tiles.Select(BillaSearchResultExtension.ToWare).ToArray();
        }

        private async Task<string> GetRequestVerificationToken(HttpClient client)
        {
            var resText = await client.GetStringAsync($"https://shop.billa.at/account/login?ReturnUrl=");
            foreach (Match match in Regex.Matches(resText, "<input.+?name\\s?=\\s?\"__RequestVerificationToken\".+?value\\s?=\\s?\"(\\S+?)\""))
            {
                return match.Groups[1].Value;
            }
            throw new Exception("Could not parse RequestVerificationToken");
        }

        private async Task<string> GetCartId(string user, string password, HttpClient client, CookieContainer cookies)
        {
            var reqtok = await GetRequestVerificationToken(client);
            var nvc = new List<KeyValuePair<string, string>>();
            nvc.Add(new KeyValuePair<string, string>("Email", user));
            nvc.Add(new KeyValuePair<string, string>("LoginPassword", password));
            nvc.Add(new KeyValuePair<string, string>("__RequestVerificationToken", reqtok));
            var req = new HttpRequestMessage(HttpMethod.Post, "https://shop.billa.at/account/login") { Content = new FormUrlEncodedContent(nvc) };
            var res = await client.SendAsync(req);
            foreach (Cookie c in cookies.GetCookies(new Uri("https://billa.at")))
            {
                if (c.Name == "CartId")
                {
                    return c.Value;
                }
            }
            throw new Exception("Could not retrieve CartId");
        }

        private async Task<JObject> GetProductInfo(HttpClient client, Ware ware)
        {
            var resText = await client.GetStringAsync($"https://shop.billa.at/api/articles/{ware.LieferantenID}?includeDetails=true");
            return JObject.Parse(resText);
        }

        public async Task WarenkorbErstellenAsync(User user, Warenkorb warenkorb)
        {
            CookieContainer cookies = new CookieContainer();
            HttpClientHandler handler = new HttpClientHandler();
            handler.CookieContainer = cookies;
            var client = new HttpClient(handler);
            var creds = await credentialEncryption.Decrypt(user.Credentials);
            var cartId = await GetCartId(creds.L_MAIL, creds.L_PASSWORD, client, cookies);
            foreach (var eintrag in warenkorb.Waren)
            {
                var info = await GetProductInfo(client, eintrag.Ware);
                info["quantity"] = eintrag.Ammount;
                var content = new StringContent($"[{info.ToString()}]", Encoding.UTF8, "application/json");
                var result = await client.PostAsync($"https://shop.billa.at/api/basket/{cartId}/upsert", content);
            }
        }
    }
}
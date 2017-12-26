using Newtonsoft.Json;

namespace BillaSkill.Models
{
    public class Warenkorb
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public string UserId { get; set; }
        public WarenkorbEintrag[] Waren { get; set; }
    }
}

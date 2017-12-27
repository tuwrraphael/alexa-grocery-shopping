using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillaSkill.Models
{
    public class Suche
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }
        public uint Position { get; set; }
        public Ware[] Waren { get; set; }
        public string SessionId { get; set; }
    }
}

using System;
using Newtonsoft.Json;

namespace ProspectManagement.Core.Models
{
    public class AddressBook
    {
        [JsonProperty("addressNumber")]
        public int AddressNumber { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("costCenter")]
        public string Area { get; set; }
    }
}

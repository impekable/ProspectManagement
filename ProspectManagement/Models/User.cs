using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Models
{
    public class User
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }
        [JsonProperty("addressNumber")]
        public int AddressNumber { get; set; }
        [JsonProperty("addressBook")]
        public AddressBook AddressBook { get; set; }
    }
}

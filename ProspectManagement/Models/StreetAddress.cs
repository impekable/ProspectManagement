using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Models
{
    public class StreetAddress
    {
        [JsonProperty("addressLine1")]
        public string AddressLine1 { get; set; }
        [JsonProperty("city")]
        public string City { get; set; }
        [JsonProperty("state")]
        public string State { get; set; }
        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }
        [JsonProperty("county")]
        public string County { get; set; }
        [JsonProperty("streetAddressVerified")]
        public bool StreetAddressVerified { get; set; }
    }
}

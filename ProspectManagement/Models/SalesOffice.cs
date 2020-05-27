using System;
using Newtonsoft.Json;

namespace ProspectManagement.Core.Models
{
    public class SalesOffice
    {
        [JsonProperty("salesOfficeId")]
        public int SalesOfficeId { get; set; }

        [JsonProperty("area")]
        public string Area { get; set; }

        [JsonProperty("salesOfficeNumber")]
        public int SalesOfficeNumber { get; set; }

        [JsonProperty("salesOfficeDescription")]
        public string SalesOfficeDescription { get; set; }

        [JsonProperty("twilioPhoneNumber")]
        public string TwilioPhoneNumber { get; set; }

        [JsonProperty("officePhoneNumber")]
        public string OfficePhoneNumber { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Models
{
    public class VisitInformation
    {
        [JsonProperty("dateCompleted")]
        public string DateCompleted { get; set; }
        [JsonProperty("notes")]
        public string Notes { get; set; }
        [JsonProperty("contactMethod")]
        public string ContactMethod { get; set; }
    }
}

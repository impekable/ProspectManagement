using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Models
{
    public class FollowUpSettings
    {
        [JsonProperty("consentToEmail")]
        public bool ConsentToEmail { get; set; }
        [JsonProperty("consentToText")]
        public bool ConsentToText { get; set; }
        [JsonProperty("consentToPhone")]
        public bool ConsentToPhone { get; set; }
        [JsonProperty("consentToMail")]
        public bool ConsentToMail { get; set; }
        [JsonProperty("excludeFromFollowup")]
        public bool ExcludeFromFollowup { get; set; }
        [JsonProperty("excludeReason")]
        public string ExcludeReason { get; set; }
        [JsonProperty("preferredContactMethod")]
        public string PreferredContactMethod { get; set; }

		public FollowUpSettings ShallowCopy()
		{
			return (FollowUpSettings)this.MemberwiseClone();
		}
    }
}

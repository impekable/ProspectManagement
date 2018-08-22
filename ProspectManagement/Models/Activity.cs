using System;
using Newtonsoft.Json;

namespace ProspectManagement.Core.Models
{
    public class Activity
    {
        [JsonProperty("instanceID")]
        public string InstanceID { get; set; }
        [JsonProperty("activityType")]
        public string ActivityType { get; set; }
        [JsonProperty("subject")]
        public string Subject { get; set; }
        [JsonProperty("notes")]
        public string Notes { get; set; }
        [JsonProperty("community")]
        public string Community { get; set; }
		[JsonProperty("timeDateStart")]
		public DateTime? TimeDateStart { get; set; }
		[JsonProperty("timeDateEnd")]
		public DateTime? TimeDateEnd { get; set; }
        [JsonProperty("dateCompleted")]
        public DateTime DateCompleted { get; set; }
        [JsonProperty("prospectAddressBook")]
        public int ProspectAddressNumber { get; set; }
        [JsonProperty("prospectCommunityId")]
        public int ProspectCommunityId { get; set; }
        [JsonProperty("salespersonAddressBook")]
        public int SalespersonAddressNumber { get; set; }
        [JsonProperty("contactMethod")]
        public string ContactMethod { get; set; }
    }
}

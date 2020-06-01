using System;
using Newtonsoft.Json;

namespace ProspectManagement.Core.Models
{
    public class SmsActivity
    {
        [JsonProperty("activityID")]
        public string ActivityId { get; set; }

        [JsonProperty("instanceID")]
        public string InstanceId { get; set; }

        [JsonProperty("messageBody")]
        public string MessageBody { get; set; }

        [JsonProperty("prospectAddressBook")]
        public int ProspectAddressBook { get; set; }

        [JsonProperty("prospectCommunityId")]
        public int ProspectCommunityId { get; set; }

        [JsonProperty("updatedDate")]
        public DateTime UpdatedDate { get; set; }

        [JsonProperty("salespersonAddressBook")]
        public int SalespersonAddressBook { get; set; }

        [JsonProperty("unreadCount")]
        public int UnreadCount { get; set; }

        [JsonProperty("unread")]
        public bool Unread { get; set; }

        [JsonProperty("direction")]
        public string Direction { get; set; }

        [JsonProperty("prospect")]
        public Prospect Prospect { get; set; }
    }

}

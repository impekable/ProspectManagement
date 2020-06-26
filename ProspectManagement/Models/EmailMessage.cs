using System;
using Newtonsoft.Json;

namespace ProspectManagement.Core.Models
{
    public class EmailMessage
    {
        [JsonProperty("e1user")]
        public string UserName { get; set; }
        [JsonProperty("from")]
        public string FromEmailAddress { get; set; }
        [JsonProperty("to")]
        public string ToEmailAddress { get; set; }
        [JsonProperty("subject")]
        public string Subject { get; set; }
        [JsonProperty("body")]
        public string Body { get; set; }
        [JsonProperty("an8")]
        public int SalespesonAddressNumber { get; set; }
        [JsonProperty("activityID")]
        public string ActivityId { get; set; }
        [JsonIgnore]
        public Action<bool> EmailCallback { get; set; }
    }
}

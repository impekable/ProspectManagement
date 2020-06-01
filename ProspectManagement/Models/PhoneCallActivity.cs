using System;
using Newtonsoft.Json;

namespace ProspectManagement.Core.Models
{
    public class PhoneCallActivity
    {
        [JsonProperty("activityID")]
        public string ActivityId { get; set; }

        [JsonProperty("instanceID")]
        public string InstanceId { get; set; }

        [JsonProperty("prospectAddressBook")]
        public int ProspectAddressBook { get; set; }

        [JsonProperty("prospectCommunityId")]
        public int ProspectCommunityId { get; set; }

        [JsonProperty("salespersonAddressBook")]
        public int SalespersonAddressBook { get; set; }

        [JsonProperty("community")]
        public string Community { get; set; }

        [JsonProperty("callTime")]
        public DateTime CallTime { get; set; }

        [JsonProperty("notes")]
        public string Notes { get; set; }

        [JsonProperty("callResult")]
        public string CallResult { get; set; }

        [JsonProperty("callSid")]
        public string CallSid { get; set; }

        [JsonProperty("callPlanId")]
        public int CallPlanId { get; set; }

        [JsonProperty("fromPhoneNumber")]
        public string FromPhoneNumber { get; set; }

        [JsonProperty("toPhoneNumber")]
        public string ToPhoneNumber { get; set; }

        [JsonProperty("phoneCallActivityID")]
        public string PhoneCallActivityId { get; set; }

        [JsonProperty("phoneCallinstanceID")]
        public string PhoneCallInstanceId { get; set; }

        [JsonProperty("followUpTaskStartDate")]
        public DateTime? FollowUpTaskStartDate { get; set; }

        [JsonProperty("followUpTaskEndDate")]
        public DateTime? FollowUpTaskEndDate { get; set; }

        [JsonProperty("followUpTaskSubject")]
        public string FollowUpTaskSubject { get; set; }
    }
}

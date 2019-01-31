using System;
using Newtonsoft.Json;

namespace ProspectManagement.Core.Models
{
    public enum EmailFormat
    {
        HTML,
        Text,
        None
    }

    public class Activity
    {
        [JsonProperty("activityID")]
        public string ActivityID { get; set; }
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
        public DateTime? DateCompleted { get; set; }
        [JsonProperty("updatedDate")]
        public DateTime? UpdatedDate { get; set; }
        [JsonProperty("prospectAddressBook")]
        public int ProspectAddressNumber { get; set; }
        [JsonProperty("prospectCommunityId")]
        public int ProspectCommunityId { get; set; }
        [JsonProperty("salespersonAddressBook")]
        public int SalespersonAddressNumber { get; set; }
        [JsonProperty("contactMethod")]
        public string ContactMethod { get; set; }
        [JsonProperty("contactType")]
        public string ContactType { get; set; }
        [JsonProperty("followUpTaskId")]
        public int FollowUpTaskId { get; set; }
        [JsonProperty("originalDueDate")]
        public DateTime? OriginalDueDate { get; set; }
        [JsonProperty("followUpStatus")]
        public string FollowUpStatus { get; set; }
        [JsonProperty("followUpMethod")]
        public string FollowUpMethod { get; set; }
        [JsonProperty("emailActivity")]
        public EmailActivityDetail EmailActivity { get; set; }
		[JsonProperty("additionalNotesExist")]
        public bool AdditionalNotesExist { get; set; }

        public ProspectCommunity ProspectCommunity { get; set; }

        public class EmailActivityDetail 
        {
            [JsonProperty("exchangeID")]
            public string ExchangeId { get; set; }
            [JsonProperty("subject")]
            public string Subject { get; set; }
            [JsonProperty("textBody")]
            public string TextBody { get; set; }
            [JsonProperty("htmlBody")]
            public string HtmlBody { get; set; }
        }
    }
}

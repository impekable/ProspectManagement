using System;
using Newtonsoft.Json;

namespace ProspectManagement.Core.Models
{
    public class ProspectCommunity
    {
        [JsonProperty("prospectCommunityId")]
        public int ProspectCommunityId { get; set; }
		[JsonProperty("leadId")]
		public int LeadId { get; set; }
        [JsonProperty("division")]
        public string Division { get; set; }
        [JsonProperty("communityNumber")]
        public string CommunityNumber { get; set; }
        [JsonProperty("addressType")]
        public string AddressType { get; set; }
        [JsonProperty("salespersonAddressNumber")]
        public int SalespersonAddressNumber { get; set; }
        [JsonProperty("salespersonName")]
        public string SalespersonName { get; set; }
        [JsonProperty("appointmentStatus")]
        public string AppointmentStatus { get; set; }
        [JsonProperty("agingStartDate")]
		public DateTime AgingStartDate { get; set; }
        [JsonProperty("lastActivityDate")]
		public DateTime LastActivityDate { get; set; }
        [JsonProperty("enteredDate")]
        public DateTime EnteredDate { get; set; }
        [JsonProperty("systemActivityDate")]
        public DateTime SystemActivityDate { get; set; }
        [JsonProperty("firstInPersonVisitDate")]
        public DateTime FirstInPersonVisitDate { get; set; }
        [JsonProperty("lastInPersonVisitDate")]
        public DateTime LastInPersonVisitDate { get; set; }
        [JsonProperty("appointmentDate")]
        public DateTime? AppointmentDate { get; set; }
        [JsonProperty("communityDetail")]
        public Community Community { get; set; }
    }
}

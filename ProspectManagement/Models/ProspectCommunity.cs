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
        [JsonProperty("visitStatus")]
        public string VisitStatus { get; set; }
        [JsonProperty("startDate")]
        public string StartDate { get; set; }
        [JsonProperty("endDate")]
        public string EndDate { get; set; }
        public Community Community { get; set; }
    }
}

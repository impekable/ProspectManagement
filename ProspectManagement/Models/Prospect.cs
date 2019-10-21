using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Models
{
	public class Prospect
	{
		[JsonProperty("prospectAddressNumber")]
		public int ProspectAddressNumber { get; set; }
		[JsonProperty("name")]
		public string Name { get; set; }
		[JsonProperty("firstName")]
		public string FirstName { get; set; }
		[JsonProperty("middleName")]
		public string MiddleName { get; set; }
		[JsonProperty("lastName")]
		public string LastName { get; set; }
		[JsonProperty("nickName")]
		public string NickName { get; set; }
		[JsonProperty("namePrefix")]
		public string NamePrefix { get; set; }
		[JsonProperty("nameSuffix")]
		public string NameSuffix { get; set; }
		[JsonProperty("email")]
		public Email Email { get; set; }
		[JsonProperty("mobilePhoneNumber")]
		public PhoneNumber MobilePhoneNumber { get; set; }
		[JsonProperty("workPhoneNumber")]
		public PhoneNumber WorkPhoneNumber { get; set; }
		[JsonProperty("homePhoneNumber")]
		public PhoneNumber HomePhoneNumber { get; set; }
		[JsonProperty("corporationIndicator")]
		public string CorporationIndicator { get; set; }
        [JsonProperty("usingRealtor")]
        public bool UsingRealtor { get; set; }
		[JsonProperty("trafficSourceCodeId")]
		public int TrafficSourceCodeId { get; set; }
		[JsonProperty("streetAddress")]
		public StreetAddress StreetAddress { get; set; }
        [JsonProperty("prospectCommunity")]
        public ProspectCommunity ProspectCommunity { get; set; }
		[JsonProperty("followUpSettings")]
		public FollowUpSettings FollowUpSettings { get; set; }
        [JsonProperty("status")]
        public string Status { get; set; }

        public Prospect ShallowCopy()
		{
			return (Prospect)this.MemberwiseClone();
		}

        public override string ToString()
        {
            return string.Format("{0} {1}", Name , Status.Equals("Inactive") ? "***Inactive***" : "");
        }
	}

}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Models
{
	public class Cobuyer
	{
		[JsonProperty("cobuyerAddressNumber")]
		public int CobuyerAddressNumber { get; set; }
		[JsonProperty("prospectAddressNumber")]
		public int ProspectAddressNumber { get; set; }
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
		[JsonProperty("addressSameAsBuyer")]
		public bool AddressSameAsBuyer { get; set; }
		[JsonProperty("streetAddress")]
		public StreetAddress StreetAddress { get; set; }

        public string FullName => LastName + ", " + FirstName;

        public Cobuyer ShallowCopy()
		{
            return (Cobuyer)this.MemberwiseClone();
		}
        		
	}

}

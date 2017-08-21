using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Models
{
    public class PhoneNumber
    {
        [JsonProperty("phoneNumber")]
        public string Phone { get; set; }
        [JsonProperty("phoneVerified")]
        public bool PhoneVerified { get; set; }
		[JsonProperty("phoneExtension")]
		public string PhoneExtension { get; set; }

		public PhoneNumber ShallowCopy()
		{
			return (PhoneNumber)this.MemberwiseClone();
		}
    }
}

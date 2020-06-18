using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Models
{
    public class Community
    {
        [JsonProperty("communityNumber")]
        public string CommunityNumber { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("internalDescription")]
        public string InternalDescription { get; set; }
        [JsonProperty("division")]
        public string Division { get; set; }
        [JsonProperty("salesOffice")]
        public SalesOffice SalesOffice { get; set; }

        public override string ToString()
		{
			return string.Format("{0}", Description);
		}
    }
}

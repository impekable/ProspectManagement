using System;
using Newtonsoft.Json;

namespace ProspectManagement.Core.Models
{
    public class UserDefinedCode
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("description1")]
        public string Description1 { get; set; }
		[JsonProperty("description2")]
		public string Description2 { get; set; }

		public override string ToString()
		{
			return string.Format("{0}", Description1);
		}
    }

}

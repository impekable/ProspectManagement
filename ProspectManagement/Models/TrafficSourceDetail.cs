using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Models
{
    public class TrafficSourceDetail
    {
        [JsonProperty("codeId")]
        public int CodeId { get; set; }
        [JsonProperty("codeDescription")]
        public string CodeDescription { get; set; }
		public override string ToString()
		{
			return string.Format("{0}", CodeDescription);
		}
    }
}

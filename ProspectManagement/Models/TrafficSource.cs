using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Models
{
    
public class TrafficSource
    {
        [JsonProperty("sourceId")]
        public int SourceId { get; set; }
        [JsonProperty("sourceDescription")]
        public string SourceDescription { get; set; }
        [JsonProperty("divisionCode")]
        public string DivisionCode { get; set; }
        [JsonProperty("trafficSourceDetails")]
        public List<TrafficSourceDetail> TrafficSourceDetails { get; set; }

		public override string ToString()
		{
			return string.Format("{0}", SourceDescription);
		}
    }
}

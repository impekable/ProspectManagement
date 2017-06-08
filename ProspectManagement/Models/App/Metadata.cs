using System;
namespace ProspectManagement.Core.Models.App
{
    public class Metadata
    {
		public string record_type { get; set; }
		public string zip_type { get; set; }
		public string county_fips { get; set; }
		public string county_name { get; set; }
		public string carrier_route { get; set; }
		public string congressional_district { get; set; }
		public string rdi { get; set; }
		public string elot_sequence { get; set; }
		public string elot_sort { get; set; }
		public double latitude { get; set; }
		public double longitude { get; set; }
		public string precision { get; set; }
		public string time_zone { get; set; }
		public int utc_offset { get; set; }
		public bool dst { get; set; }
    }
}

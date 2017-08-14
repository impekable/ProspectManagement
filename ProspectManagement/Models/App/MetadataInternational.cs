using System;
namespace ProspectManagement.Core.Models.App
{
    public class MetadataInternational
    {
		public double latitude { get; set; }
		public double longitude { get; set; }
		public string geocode_precision { get; set; }
		public string max_geocode_precision { get; set; }
		public string address_format { get; set; }
    }
}

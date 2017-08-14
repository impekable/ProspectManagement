using System;
namespace ProspectManagement.Core.Models.App
{
    public class StreetAddressInternationalValidationResult
    {
		public string address1 { get; set; }
		public string address2 { get; set; }
		public string address3 { get; set; }
		public string address4 { get; set; }
		public ComponentsInternational components { get; set; }
		public MetadataInternational metadata { get; set; }
		public AnalysisInternational analysis { get; set; }
    }
}

using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ProspectManagement.Core.Models.App
{
	public class ServiceError
	{
		[JsonProperty("error")]
		public string Error { get; set; }
		[JsonProperty("error_description")]
		public string ErrorDescription { get; set; }
		[JsonProperty("error_codes")]
		public List<string> ErrorCodes { get; set; }
		[JsonProperty("timestamp")]
		public DateTime TimeStamp { get; set; }
	}
}

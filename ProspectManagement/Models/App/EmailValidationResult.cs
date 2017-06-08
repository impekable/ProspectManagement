using System;
using Newtonsoft.Json;

namespace ProspectManagement.Core.Models
{
	public class EmailValidationResult
	{
		[JsonProperty("address")]
		public string Address
		{
			get;
			set;
		}
		[JsonProperty("did_you_mean")]
		public string DidYouMean
		{
			get;
			set;
		}
		[JsonProperty("is_valid")]
		public bool IsValid
		{
			get;
			set;
		}
		[JsonProperty("parts")]
		public EmailParts EmailParts
		{
			get;
			set;
		}
	}
}

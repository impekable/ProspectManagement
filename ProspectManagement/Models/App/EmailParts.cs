using System;
using Newtonsoft.Json;

namespace ProspectManagement.Core.Models
{
	public class EmailParts
	{
		[JsonProperty("display_name")]
		public string DisplayName
		{
			get;
			set;
		}
		[JsonProperty("domain")]
		public string Domain
		{
			get;
			set;
		}
		[JsonProperty("local_part")]
		public string LocalPart
		{
			get;
			set;
		}
	}
}

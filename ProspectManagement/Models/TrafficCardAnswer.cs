using System;
using Newtonsoft.Json;

namespace ProspectManagement.Core.Models
{
	public class TrafficCardAnswer
	{
		[JsonProperty("answerNumber")]
		public int AnswerNumber { get; set; }
		[JsonProperty("answerSequenceNumber")]
		public int AnswerSequenceNumber { get; set; }
		[JsonProperty("answerText")]
		public string AnswerText { get; set; }
		[JsonProperty("weightingValue")]
		public int WeightingValue { get; set; }
		public override string ToString()
		{
			return string.Format("{0}", AnswerText);
		}
	}
}

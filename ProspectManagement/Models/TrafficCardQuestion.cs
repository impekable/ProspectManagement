using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ProspectManagement.Core.Models
{
	public class TrafficCardQuestion
	{
		[JsonProperty("questionNumber")]
		public int QuestionNumber { get; set; }
		[JsonProperty("questionSequenceNumber")]
		public int QuestionSequenceNumber { get; set; }
		[JsonProperty("weightingValue")]
		public int WeightingValue { get; set; }
		[JsonProperty("questionText")]
		public string QuestionText { get; set; }
		[JsonProperty("trafficCardAnswers")]
		public List<TrafficCardAnswer> TrafficCardAnswers { get; set; }
	}
}

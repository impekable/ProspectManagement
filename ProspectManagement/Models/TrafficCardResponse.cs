using System;
using System.Linq;
using Newtonsoft.Json;

namespace ProspectManagement.Core.Models
{
    public class TrafficCardResponse
    {
        [JsonProperty("responseNumber")]
        public int ResponseNumber { get; set; }
        [JsonProperty("answerNumber")]
        public int AnswerNumber { get; set; }
        [JsonProperty("comments")]
        public string Comments { get; set; }
        [JsonProperty("trafficCardQuestion")]
        public TrafficCardQuestion TrafficCardQuestion { get; set; }
        public string AnswerText
        {
            get
            {
                var answer = TrafficCardQuestion.TrafficCardAnswers.FirstOrDefault(a => a.AnswerNumber == AnswerNumber);
                return answer == null ? "N/A" : answer.AnswerText;
            }
        }
    }
}

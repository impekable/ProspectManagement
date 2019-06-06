using System;
using Newtonsoft.Json;

namespace ProspectManagement.Core.Models
{
    public class BuyerDecisions
    {
        [JsonProperty("prospectAddressNumber")]
        public int ProspectAddressNumber { get; set; }

        [JsonProperty("unsatisified")]
        public bool Unsatisified { get; set; }

        [JsonProperty("home")]
        public bool Home { get; set; }

        [JsonProperty("market")]
        public bool Market { get; set; }

        [JsonProperty("builder")]
        public bool Builder { get; set; }

        [JsonProperty("area")]
        public bool Area { get; set; }

        [JsonProperty("community")]
        public bool Community { get; set; }

        [JsonProperty("homeSite")]
        public bool HomeSite { get; set; }

        [JsonProperty("circumstantialUrgency")]
        public bool CircumstantialUrgency { get; set; }

        [JsonProperty("economicClimate")]
        public bool EconomicClimate { get; set; }

        [JsonProperty("financing")]
        public bool Financing { get; set; }

        [JsonProperty("xFactor")]
        public bool XFactor { get; set; }

        [JsonProperty("finalDecision")]
        public bool FinalDecision { get; set; }

        [JsonProperty("secondFinalDecision")]
        public bool SecondFinalDecision { get; set; }

        [JsonProperty("systemRanking")]
        public string SystemRanking { get; set; }

        [JsonProperty("ranking")]
        public string Ranking { get; set; }
    }
}


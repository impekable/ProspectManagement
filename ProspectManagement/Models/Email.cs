using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Models
{
    public class Email
    {
        [JsonProperty("email")]
        public string EmailAddress { get; set; }
        [JsonProperty("emailVerified")]
        public bool EmailVerified { get; set; }
    }

}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Models
{
    public class User
    {
        [JsonProperty("userId")]
        public string UserId { get; set; }
        [JsonProperty("addressNumber")]
        public int AddressNumber { get; set; }
        [JsonProperty("addressBook")]
        public AddressBook AddressBook { get; set; }
        [JsonProperty("mobilePhoneNumber")]
        public PhoneNumber MobilePhoneNumber { get; set; }
        [JsonProperty("officePhoneNumber")]
        public PhoneNumber OfficePhoneNumber { get; set; }
        [JsonProperty("email")]
        public Email Email { get; set; }
        [JsonProperty("unreadSMSCount")]
        public int UnreadSMSCount { get; set; }
        [JsonProperty("upcomingAppointmentCount")]
        public int UpcomingAppointmentCount { get; set; }
        [JsonProperty("missedCallCount")]
        public int MissedCallCount { get; set; }
        [JsonProperty("usingTelephony")]
        public bool UsingTelephony { get; set; }
    }
}

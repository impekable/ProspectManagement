using System;
namespace ProspectManagement.Core.Models.App
{
    public class TwilioCallParameters
    {
        public string AccessToken { get; set; }
        public string ToPhoneNumber { get; set; }
        public string FromPhoneNumber { get; set; }
    }
}

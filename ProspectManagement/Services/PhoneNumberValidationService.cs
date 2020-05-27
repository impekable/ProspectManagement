using System;
using System.Threading.Tasks;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;
using ProspectManagement.Core.Repositories;

namespace ProspectManagement.Core.Services
{
	public class PhoneNumberValidationService : BaseRepository, IPhoneNumberValidationService
	{
		protected const string _twilioUri = "https://lookups.twilio.com/v1/PhoneNumbers/";

		public async Task<bool> Validate(PhoneNumber phoneNumber)
		{
			var verified = await GetResultFromAPI(_twilioUri+phoneNumber.Phone, Constants.PrivateKeys.TwilioAccountSid, Constants.PrivateKeys.TwilioAuthToken);
			phoneNumber.PhoneVerified = verified;

			return verified;
		}

	}
}

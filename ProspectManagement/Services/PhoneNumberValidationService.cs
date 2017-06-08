using System;
using System.Threading.Tasks;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;
using ProspectManagement.Core.Repositories;

namespace ProspectManagement.Core.Services
{
	public class PhoneNumberValidationService : BaseRepository, IPhoneNumberValidationService
	{
		const string accountSid = "AC73d4e74676995cdfb713ba4e41f58f95";
		const string authToken = "d22a80c12bc2cfed0d9012e1e1ea4aff";
		protected const string _twilioUri = "https://lookups.twilio.com/v1/PhoneNumbers/";

		public async Task<bool> Validate(PhoneNumber phoneNumber)
		{
			var verified = await GetResultFromAPI(_twilioUri+phoneNumber.Phone, accountSid, authToken);
			phoneNumber.PhoneVerified = verified;

			return verified;
		}

	}
}

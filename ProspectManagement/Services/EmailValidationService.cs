using System;
using System.Threading.Tasks;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;
using ProspectManagement.Core.Repositories;

namespace ProspectManagement.Core.Services
{
	public class EmailValidationService: BaseRepository, IEmailValidationService
	{
		const string pubkey = "pubkey-158b8ca3bef9268b931533281b59ec35";
		protected const string _mailgunUri = "https://api.mailgun.net/v3/address/validate?address={0}";

		public async Task<EmailValidationResult> Validate(Email email)
		{
			var emailValidationResult = await GetDataObjectFromAPI<EmailValidationResult>(string.Format(_mailgunUri, email.EmailAddress), "api", pubkey);
			email.EmailVerified = emailValidationResult.IsValid;

			return emailValidationResult;
		}
	}
}

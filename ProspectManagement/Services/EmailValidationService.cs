using System;
using System.Threading.Tasks;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;
using ProspectManagement.Core.Repositories;

namespace ProspectManagement.Core.Services
{
	public class EmailValidationService: BaseRepository, IEmailValidationService
	{
        const string pubkey = "pubkey-265011f5238d3f1e5adf310b73430c07"; //unpaid account
        //const string pubkey = "pubkey-158b8ca3bef9268b931533281b59ec35"; //paid account

		protected const string _mailgunUri = "https://api.mailgun.net/v3/address/validate?address={0}";

		public async Task<EmailValidationResult> Validate(Email email)
		{
			var emailValidationResult = await GetDataObjectFromAPI<EmailValidationResult>(string.Format(_mailgunUri, email.EmailAddress), "api", pubkey);
            email.EmailVerified = emailValidationResult != null ? emailValidationResult.IsValid : false;

            if (emailValidationResult != null)
            {
                email.EmailVerified = emailValidationResult.IsValid;
                return emailValidationResult;
            }
            else
            {
                email.EmailVerified = false;
                return new EmailValidationResult() { IsValid = true };
            }
        }
	}
}

using System;
using System.Threading.Tasks;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Interfaces.Services
{
	public interface IEmailValidationService
	{
		Task<EmailValidationResult> Validate(Email email);
	}
}

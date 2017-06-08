using System;
using System.Threading.Tasks;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Interfaces.Services
{
	public interface IPhoneNumberValidationService
	{
		Task<bool> Validate(PhoneNumber phoneNumber);
	}
}

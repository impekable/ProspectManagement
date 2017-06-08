using System;
using System.Threading.Tasks;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Interfaces.Services
{
	public interface IStreetValidationService
	{
		Task<bool> Validate(StreetAddress streetAddress);
	}
}

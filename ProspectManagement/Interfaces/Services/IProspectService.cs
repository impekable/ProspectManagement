using System;
using System.Collections.Generic;
using ProspectManagement.Core.Models;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Interfaces.Services
{
	public interface IProspectService
	{
        Task<List<Prospect>> GetProspectsAsync(string accessToken, List<Community> communities, int? salespersonId, string type, int page, int pageSize, string searchTerm);

		Task<Prospect> GetProspectAsync(int prospectId);

		Task<bool> UpdateProspectAsync(Prospect prospect);

		Task<AddressBook> AssignProspectToLoggedInUserAsync(string communityNumber, int prospectId);
	}
}

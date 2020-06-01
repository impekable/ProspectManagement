using System;
using System.Collections.Generic;
using ProspectManagement.Core.Models;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Interfaces.Services
{
	public interface IProspectService
	{
        Task<List<Prospect>> GetProspectsAsync(string accessToken, List<Community> communities, int? salespersonId, string type, int page, int pageSize, string searchTerm, string searchTermScope);

		Task<Prospect> GetProspectAsync(int prospectId);

		Task<List<SmsActivity>> GetProspectSMSActivityAsync(int prospectId, string accessToken = null, int page = 1, int pageSize = 20);

		Task<bool> UpdateProspectAsync(Prospect prospect);

		Task<bool> UpdateProspectSMSActivityAsync(int prospectId);

		Task<AddressBook> AssignProspectToLoggedInUserAsync(string communityNumber, int prospectId);
	}
}

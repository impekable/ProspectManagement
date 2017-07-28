using System;
using System.Collections.Generic;
using ProspectManagement.Core.Models;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Interfaces.Services
{
	public interface IProspectService
	{
		Task<List<Prospect>> GetProspectsAsync(List<Community> communities, int? salespersonId, int page, int pageSize, string searchTerm);

		Task<Prospect> GetProspectAsync(int prospectId);

		Task<bool> UpdateProspectAsync(Prospect prospect);

		Task<bool> AssignProspectToLoggedInUserAsync(string communityNumber, int prospectId);
	}
}

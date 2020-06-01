using System;
using System.Collections.Generic;
using ProspectManagement.Core.Models;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Interfaces.Repositories
{
	public interface IProspectRepository: IBaseRepository
	{
        Task<List<Prospect>> GetProspectsAsync(string accessToken, int? salespersonId, string type, List<Community> communities, int page, int pageSize, string searchTerm, string searchTermScope);

		Task<Prospect> GetProspectAsync(int prospectId, string accessToken);

		Task<List<SmsActivity>> GetProspectSMSActivityAsync(int prospectId, string accessToken, int page, int pageSize);

		Task<bool> UpdateProspectAsync(Prospect prospect, string accessToken);

		Task<object> AssignProspectToSalespersonAsync(string communityNumber, int prospectId, int salespersonId, string accessToken);

		Task<bool> UpdateProspectSMSActivityAsync(int prospectId, string accessToken);
	}
}

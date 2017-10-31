using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Interfaces.Repositories
{
	public interface ITrafficCardResponseRepository : IBaseRepository
	{
		Task<List<TrafficCardResponse>> GetTrafficCardResponsesForProspectAsync(int prospectAddressNumber, bool requiredOnly, string accessToken);
		Task<bool> UpdateTrafficCardResponseAsync(int prospectAddressNumber, List<TrafficCardResponse> response, string accessToken);
	}	
}

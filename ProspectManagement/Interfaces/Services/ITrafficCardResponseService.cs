using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Interfaces.Services
{
	public interface ITrafficCardResponseService
	{
		Task<bool> UpdateTrafficCardResponse(int prospectAddressNumber, List<TrafficCardResponse> response);
		Task<List<TrafficCardResponse>> GetTrafficCardResponsesForProspect(int prospectAddressNumber, bool requiredOnly);
	}
}

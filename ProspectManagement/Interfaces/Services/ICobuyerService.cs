using System;
using System.Collections.Generic;
using ProspectManagement.Core.Models;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Interfaces.Services
{
	public interface ICobuyerService
	{
		Task<List<Cobuyer>> GetCobuyersForProspectAsync(Prospect prospect);

        Task<Cobuyer> GetCobuyerAsync(int prospectId, int cobuyerId, string authToken);

		Task<bool> UpdateCobuyerAsync(Cobuyer cobuyer);

		Task<Cobuyer> AddCobuyerToProspectAsync(int prospectId, Cobuyer cobuyer);

		Task<bool> DeleteCobuyerFromProspectAsync(int cobuyerId);
	}
}

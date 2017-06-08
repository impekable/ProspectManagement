using System;
using System.Collections.Generic;
using ProspectManagement.Core.Models;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Interfaces.Services
{
	public interface ICobuyerService
	{
		Task<List<Cobuyer>> GetCobuyersForProspectAsync(int prospectId);

		Task<Cobuyer> GetCobuyerAsync(int prospectId, int cobuyerId);

		Task<bool> UpdateCobuyerAsync(Cobuyer cobuyer);

		Task<Cobuyer> AddCobuyerToProspectAsync(int prospectId, Cobuyer cobuyer);

		Task<bool> DeleteCobuyerFromProspectAsync(int cobuyerId);
	}
}

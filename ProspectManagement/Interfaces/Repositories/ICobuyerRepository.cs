using System;
using System.Collections.Generic;
using ProspectManagement.Core.Models;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Interfaces.Repositories
{
	public interface ICobuyerRepository: IBaseRepository
	{
		Task<List<Cobuyer>> GetCobuyersForProspectAsync(int prospectId, string accessToken);

		Task<Cobuyer> GetCobuyerAsync(int prospectId ,int cobuyerId, string accessToken);

		Task<bool> UpdateCobuyerAsync(Cobuyer cobuyer, string accessToken);

        Task<Cobuyer> AddCobuyerToProspectAsync(int prospectId, Cobuyer cobuyer, string accessToken);

		Task<bool> DeleteCobuyerFromProspectAsync(int cobuyerId, string accessToken);
	}
}

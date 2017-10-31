using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Repositories
{
    public class CobuyerRepository: BaseRepository, ICobuyerRepository
    {
        const string _baseUri = _devUri + "Prospects/{0}/Cobuyers/";

        public async Task<Cobuyer> AddCobuyerToProspectAsync(int prospectId, Cobuyer cobuyer, string accessToken)
        {
            return await PostDataObjectToAPI(string.Format(_baseUri, prospectId), cobuyer, accessToken);
        }

        public async Task<Cobuyer> GetCobuyerAsync(int prospectId, int cobuyerId)
        {
            return await GetDataObjectFromAPI<Cobuyer>(string.Format(_baseUri, prospectId) + cobuyerId, "");
        }

        public async Task<List<Cobuyer>> GetCobuyersForProspectAsync(int prospectId)
        {
            return await GetDataObjectFromAPI <List<Cobuyer>> (string.Format(_baseUri, prospectId), "");
        }

		public Task<bool> DeleteCobuyerFromProspectAsync(int cobuyerId, string accessToken)
		{
			throw new NotImplementedException();
		}

        public Task<bool> UpdateCobuyerAsync(Cobuyer cobuyer, string accessToken)
        {
            throw new NotImplementedException();
        }
    }
}

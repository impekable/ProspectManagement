using System;
using System.Threading.Tasks;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Repositories
{
    public class BuyerDecisionsRepository : BaseRepository, IBuyerDecisionsRepository
    {
        const string _baseUri = _e1Uri + "BuyerDecisions/{0}";

        public async Task<BuyerDecisions> GetBuyerDecisionsAsync(int prospectId, string accessToken)
        {
            return await GetDataObjectFromAPI<BuyerDecisions>(string.Format(_baseUri, prospectId), accessToken);
        }

        public async Task<bool> UpdateBuyerDecisionsAsync(BuyerDecisions decisions, string accessToken)
        {
            return await PutDataObjectToAPI(string.Format(_baseUri, decisions.ProspectAddressNumber), decisions, accessToken);
        }
    }
}

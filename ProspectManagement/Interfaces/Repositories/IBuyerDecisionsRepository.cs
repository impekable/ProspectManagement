using System;
using System.Threading.Tasks;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Interfaces.Repositories
{
    public interface IBuyerDecisionsRepository : IBaseRepository
    {
        Task<BuyerDecisions> GetBuyerDecisionsAsync(int prospectId, string accessToken);

        Task<bool> UpdateBuyerDecisionsAsync(BuyerDecisions decisions, string accessToken);

    }
}

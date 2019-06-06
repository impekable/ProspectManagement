using System;
using System.Threading.Tasks;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Interfaces.Services
{
    public interface IBuyerDecisionsService
    {
        Task<BuyerDecisions> GetBuyerDecisionsAsync(int prospectId);

        Task<bool> UpdateBuyerDecisionsAsync(BuyerDecisions decisions);
    }
}

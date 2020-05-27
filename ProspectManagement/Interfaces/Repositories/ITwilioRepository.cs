using System;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Interfaces.Repositories
{
    public interface ITwilioRepository : IBaseRepository
    {
        Task<string> GetClientToken(string userId);
    }
}

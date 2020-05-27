using System;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Interfaces.Services
{
    public interface ITwilioService
    {
        Task<String> GetClientToken(string userId);
    }
}

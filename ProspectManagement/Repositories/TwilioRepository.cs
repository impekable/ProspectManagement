using System;
using System.Threading.Tasks;
using ProspectManagement.Core.Interfaces.Repositories;

namespace ProspectManagement.Core.Repositories
{
    public class TwilioRepository : BaseRepository, ITwilioRepository
    {
        const string _baseUri = "https://optoutdv.khov.com/E1CRMWebApp/Client/Token?" + "identity={0}";

        public async Task<string> GetClientToken(string userId)
        {
            return await GetFromAPI(string.Format(_baseUri, userId));
        }
    }
}

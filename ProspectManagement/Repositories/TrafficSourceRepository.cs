using ProspectManagement.Core.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Repositories
{
    public class TrafficSourceRepository : BaseRepository, ITrafficSourceRepository
    {
        const string _baseUri = _devUri + "TrafficSources"; 

        public async Task<TrafficSource> GetTrafficSourceDetailsAsync(int sourceId, string accessToken)
        {
            return await GetDataObjectFromAPI<TrafficSource>(string.Format(_baseUri + "/{0}?Fields=trafficSourceDetails", sourceId), accessToken);
        }

        public async Task<List<TrafficSource>> GetTrafficSourcesByDivisionAsync(string divisionCode, string accessToken)
        {
            return await GetDataObjectFromAPI<List<TrafficSource>>(string.Format(_baseUri + "?Division={0}&Fields=trafficSourceDetails", divisionCode), accessToken);
        }

    }
}

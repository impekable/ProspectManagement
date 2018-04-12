using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Repositories
{
    public class ActivityRepository: BaseRepository, IActivityRepository
    {
        const string _baseUri = _e1Uri + "Prospects/{0}/Activity/";

        public async Task<Activity> AddProspectActivityAsync(int prospectId, Activity activity, string accessToken)
        {
            return await PostDataObjectToAPI(string.Format(_baseUri, prospectId), activity, accessToken);
        }

        public async Task<List<Activity>> GetProspectActivitiesAsync(string accessToken, int prospectId, int page, int pageSize)
        {
            return await GetDataObjectFromAPI<List<Activity>>(string.Format(_baseUri, prospectId), accessToken);
        }
    }
}

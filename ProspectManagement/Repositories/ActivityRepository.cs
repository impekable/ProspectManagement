using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Repositories
{
    public class ActivityRepository: BaseRepository, IActivityRepository
    {
        const string _baseUri = _e1Uri + "Prospects/{0}/";
        const string _emailUri = "?Fields=emailActivity";
        const string _emailHTMLUri = "?Fields=emailActivity.htmlBody";

        public async Task<Activity> AddProspectActivityAsync(int prospectId, Activity activity, string accessToken)
        {
            return await PostDataObjectToAPI(string.Format(_baseUri + "Activity", prospectId), activity, accessToken);
        }

        public async Task<List<Activity>> GetProspectActivitiesAsync(int prospectId, EmailFormat emailFormat, string accessToken)
        {
            string _Uri = _baseUri + "Activities";

            if (emailFormat == EmailFormat.HTML)
            {
                _Uri += _emailHTMLUri; 
            }
            else if (emailFormat == EmailFormat.Text)
            {
                _Uri += _emailUri;
            }

            return await GetDataObjectFromAPI<List<Activity>>(string.Format(_Uri, prospectId), accessToken);
        }

        public async Task<Activity> GetProspectActivityAsync(int prospectId, string instanceId, EmailFormat emailFormat, string accessToken)
        {
            string _Uri = _baseUri + "Activity/{1}";

            if (emailFormat == EmailFormat.HTML)
            {
                _Uri += _emailHTMLUri;
            }
            else if (emailFormat == EmailFormat.Text)
            {
                _Uri += _emailUri;
            }

            return await GetDataObjectFromAPI<Activity>(string.Format(_Uri, prospectId, instanceId), accessToken);
        }
    }
}


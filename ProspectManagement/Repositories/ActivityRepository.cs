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

        public async Task<Activity> LogCallAsync(int prospectId, Activity activity, string accessToken)
        {
            return await PostDataObjectToAPI(string.Format(_baseUri + "LogCall", prospectId), activity, accessToken);
        }

        public async Task<Activity> GetActivityWithTemplateDataAsync(int prospectId, string instanceId, string templateType, string accessToken)
        {
            return await GetDataObjectFromAPI<Activity>(string.Format(_baseUri + "Activity/{1}/Template?TemplateType={2}", prospectId, instanceId, templateType), accessToken);
        }

        public async Task<List<Activity>> GetDailyToDoActivitiesAsync(string accessToken, int salespersonId, string category, DateTime dueAsOfDate, int page = 1, int pageSize = 20)
        {
            return await GetDataObjectFromAPI<List<Activity>>(_e1Uri + $"Salesperson/{salespersonId}/FollowUp?Statuses=Pending,Executed&Fields=prospectCommunity,emailSubject,ranking&DueDate={dueAsOfDate.ToShortDateString()}& CommunityList=&Page={page}&PageSize={pageSize}&Category={category}", accessToken);
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

        public async Task<List<SmsActivity>> GetSmsActivitiesAsync(string accessToken, int salespersonId, bool newOnly, int page, int pageSize)
        {
            return await GetDataObjectFromAPI<List<SmsActivity>>(_e1Uri + $"Salesperson/{salespersonId}/SMSActivity?Fields=prospectCommunity&NewOnly={newOnly}&Page={page}&PageSize={pageSize}", accessToken);
        }

        public async Task<bool> UpdateActivityForProspectAsync(Activity activity, string accessToken)
        {
            return await PutDataObjectToAPI(string.Format(_baseUri + "Activity/{1}", activity.ProspectAddressNumber, activity.InstanceID), activity, accessToken);
        }

        public async Task<PhoneCallActivity> AddAdHocPhoneCallActivityAsync(int prospectId, PhoneCallActivity activity, string accessToken)
        {
            return await PostDataObjectToAPI(string.Format(_baseUri + "PhoneCallActivity", prospectId), activity, accessToken);
        }

        public async Task<bool> UpdatePhoneCallActivityForProspectAsync(PhoneCallActivity activity, string accessToken)
        {
            return await PutDataObjectToAPI(string.Format(_baseUri + "PhoneCallActivity/{1}", activity.ProspectAddressBook, activity.InstanceId), activity, accessToken);
        }

        public async Task<bool> SendEmail(int prospectId, EmailMessage email, string accessToken)
        {
            return await PutDataObjectToAPI(string.Format(_baseUri + "Email", prospectId), email, accessToken);
        }
    }
}


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Interfaces.Repositories
{
    public interface IActivityRepository: IBaseRepository
    {
        Task<List<Activity>> GetProspectActivitiesAsync(int prospectId, EmailFormat emailFormat, string accessToken);

        Task<Activity> AddProspectActivityAsync(int prospectId, Activity activity, string accessToken);

        Task<PhoneCallActivity> AddAdHocPhoneCallActivityAsync(int prospectId, PhoneCallActivity activity, string accessToken);

        Task<bool> UpdatePhoneCallActivityForProspectAsync(PhoneCallActivity activity, string accessToken);

        Task<Activity> LogCallAsync(int prospectId, Activity activity, string accessToken);

        Task<Activity> GetProspectActivityAsync(int prospectId, string activityId, EmailFormat emailFormat, string accessToken);

        Task<List<Activity>> GetDailyToDoActivitiesAsync(string accessToken, int salespersonId, string category, DateTime dueAsOfDate, int page, int pageSize);

        Task<bool> UpdateActivityForProspectAsync(Activity activity, string accessToken);

        Task<bool> SendEmail(int prospectId, EmailMessage email, string accessToken);

        Task<Activity> GetActivityWithTemplateDataAsync(int prospectId, string instanceId, string templateType, string accessToken);

        Task<List<SmsActivity>> GetSmsActivitiesAsync(string accessToken, int salespersonId, bool newOnly, int page, int pageSize);

    }
}

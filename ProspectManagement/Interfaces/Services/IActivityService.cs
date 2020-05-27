using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Interfaces.Services
{
    public interface IActivityService
    {
        Task<List<Activity>> GetActivitiesForProspectAsync(int prospectId);

        Task<Activity> AddActivityToProspectAsync(int prospectId, Activity activity);

        Task<Activity> LogCallAsync(int prospectId, Activity activity);

        Task<PhoneCallActivity> AddAdHocPhoneCallActivityAsync(int prospectId, PhoneCallActivity activity);

        Task<bool> UpdatePhoneCallActivityForProspectAsync(PhoneCallActivity activity);

        Task<Activity> GetActivityForProspectAsync(int prospectId, string activityId);

        Task<Activity> AddNoteToProspectAsync(Prospect prospect, String note);

        Task<Activity> AddVisitToProspectAsync(Prospect prospect, String note);

        Task<List<Activity>> GetDailyToDoActivitiesAsync(string accessToken, List<UserDefinedCode> categories, int salespersonId, string category, DateTime dueAsOfDate, int page, int pageSize);

        Task<bool> UpdateActivityForProspectAsync(Activity activity);

        Task<Activity> GetActivityWithTemplateDataAsync(int prospectId, string instanceId, string templateType);

        Task<bool> SendSMSAsync(Activity activity);

        Task<Activity> SendAdHocSMSAsync(int prospectId, Activity activity);

        Task<List<SmsActivity>> GetSmsActivitiesAsync(string accessToken, int salespersonId, bool newOnly, int page, int pageSize);

    }
}

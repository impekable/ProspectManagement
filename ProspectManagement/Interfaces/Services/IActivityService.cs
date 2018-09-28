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

        Task<Activity> GetActivityForProspectAsync(int prospectId, string activityId);

        Task<Activity> AddNoteToProspectAsync(Prospect prospect, String note);

        Task<Activity> AddVisitToProspectAsync(Prospect prospect, String note);
    }
}

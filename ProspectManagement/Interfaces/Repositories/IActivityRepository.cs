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

        Task<Activity> GetProspectActivityAsync(int prospectId, string activityId, EmailFormat emailFormat, string accessToken);

    }
}

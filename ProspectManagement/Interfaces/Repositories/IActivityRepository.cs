using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProspectManagement.Core.Models;

namespace ProspectManagement.Core.Interfaces.Repositories
{
    public interface IActivityRepository: IBaseRepository
    {
        Task<List<Activity>> GetProspectActivitiesAsync(string accessToken, int prospectId, int page, int pageSize);

        Task<Activity> AddProspectActivityAsync(int prospectId, Activity activity, string accessToken);

    }
}

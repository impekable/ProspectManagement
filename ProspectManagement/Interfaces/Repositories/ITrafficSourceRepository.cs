using ProspectManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Interfaces.Repositories
{
    public interface ITrafficSourceRepository : IBaseRepository
    {
        Task<List<TrafficSource>> GetTrafficSourcesByDivisionAsync(string divisionCode);

        Task<TrafficSource> GetTrafficSourceDetailsAsync(int sourceId);
    }
}

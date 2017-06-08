using ProspectManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Interfaces.Services
{
    public interface ITrafficSourceService
    {
        Task<List<TrafficSource>> GetTrafficSourcesByDivision(string divisionCode);

        Task<TrafficSource> GetTrafficSourceDetails(int sourceId);

        Task<List<TrafficSource>> GetTrafficSourcesForDefaultCommunity();
    }
}

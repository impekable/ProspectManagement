using ProspectManagement.Core.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProspectManagement.Core.Models;
using ProspectManagement.Core.Interfaces.Repositories;

namespace ProspectManagement.Core.Services
{
    public class TrafficSourceService : ITrafficSourceService
    {
        private readonly ITrafficSourceRepository _trafficSourceRepository;
        private readonly IDefaultCommunityRepository _defaultCommunityRepository;
        public TrafficSourceService(IDefaultCommunityRepository defaultCommunityRepository, ITrafficSourceRepository trafficSourceRepository)
        {
            _defaultCommunityRepository = defaultCommunityRepository;
            _trafficSourceRepository = trafficSourceRepository;
        }
        public async Task<TrafficSource> GetTrafficSourceDetails(int sourceId)
        {
            return await _trafficSourceRepository.GetTrafficSourceDetailsAsync(sourceId);
        }

        public async Task<List<TrafficSource>> GetTrafficSourcesByDivision(string divisionCode)
        {
            return await _trafficSourceRepository.GetTrafficSourcesByDivisionAsync(divisionCode);
        }

        public async Task<List<TrafficSource>> GetTrafficSourcesForDefaultCommunity()
        {
            var community = await _defaultCommunityRepository.GetDefaultCommunity();
			return await _trafficSourceRepository.GetTrafficSourcesByDivisionAsync(community.Division);
        }
    }
}

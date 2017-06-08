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
    public class CommunityService : ICommunityService
    {
        private readonly ICommunityRepository _communityRepository;

        public CommunityService(ICommunityRepository communityRepository)
        {
            _communityRepository = communityRepository;
        }

        public async Task<List<Community>> GetCommunitiesByDivision(string divisionCode, bool activeOnly)
        {
            return await _communityRepository.GetCommunitiesByDivisionAsync(divisionCode, activeOnly);
        }

        public async Task<List<Community>> GetCommunitiesBySalesperson(int salespersonId)
        {
            return await _communityRepository.GetCommunitiesBySalespersonAsync(salespersonId);
        }
    }
}

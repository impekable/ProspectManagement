using ProspectManagement.Core.Interfaces.Repositories;
using ProspectManagement.Core.Interfaces.Services;
using ProspectManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Services
{
    public class DefaultCommunityService : IDefaultCommunityService
    {
        private readonly IDefaultCommunityRepository _defaultCommunityRepository;
        public DefaultCommunityService(IDefaultCommunityRepository defaultCommunityRepository)
        {
            _defaultCommunityRepository = defaultCommunityRepository;
        }
        public Task<Community> GetDefaultCommunity()
        {
            return _defaultCommunityRepository.GetDefaultCommunity();
        }

        public void SaveDefaultCommunity(Community community)
        {
            _defaultCommunityRepository.SaveDefaultCommunity(community);
        }
    }
}

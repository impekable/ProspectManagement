using ProspectManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Interfaces.Services
{
    public interface IDefaultCommunityService
    {
        Task<Community> GetDefaultCommunity();
        void SaveDefaultCommunity(Community community);
    }
}

using ProspectManagement.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProspectManagement.Core.Interfaces.Services
{
    public interface ICommunityService
    {
        Task<List<Community>> GetCommunitiesBySalesperson(int salespersonId);

        Task<List<Community>> GetCommunitiesByDivision(string divisionCode, bool activeOnly);
    }
}
